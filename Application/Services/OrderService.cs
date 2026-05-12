using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository,
        IProductRepository productRepository, ICustomerRepository customerRepository,
        IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PagedResponse<OrderResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching orders. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new PagedResponse<OrderResponse>();
        }

        var orders = await _orderRepository.GetAllAsync(pageNumber,pageSize);
        
        var items = orders.Items.Select(o => new OrderResponse()
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            EmployeeId = o.EmployeeId,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            Items = o.OrderItems.Select(i => new OrderItemResponse()
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.Quantity * i.Price
            }).ToList()

        }).ToList();

        _logger.LogInformation("Returned {Count} orders out of {Total}", items.Count, orders.TotalCount);
        
        return new PagedResponse<OrderResponse>()
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = orders.TotalCount
        };
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        if (request.Items == null || request.Items.Count <= 0)
        {
            throw new InvalidOrderException("Order must contain at least one item");
        }
        
        var hasDuplicates = request.Items
            .GroupBy(x => x.ProductId)
            .Any(g => g.Count() > 1);
        
        if (hasDuplicates)
        {
            throw new DuplicateProductException();
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }

        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.EmployeeId);
            throw new EmployeeNotFoundException(request.EmployeeId);
        }
        
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            _logger.LogInformation("Creating order using Customer ID: {CustomerId}", request.CustomerId);
            var order = new Order
            {
                CustomerId = request.CustomerId,
                EmployeeId = request.EmployeeId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };
                    
            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found while creating order", item.ProductId);
                    throw new ProductNotFoundException(item.ProductId);
                }
            
                if (item.Quantity <= 0)
                {
                    _logger.LogWarning("Invalid quantity for product {ProductId}", item.ProductId);
                    throw new InvalidQuantityException(item.ProductId);
                }
            
                if (item.Quantity > product.Stock)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId}. Available: {Stock}, Requested: {Quantity}",
                        item.ProductId, product.Stock, item.Quantity);
                    throw new InsufficientStockException(item.ProductId, product.Stock, item.Quantity);
                }
                
                var orderItems = new OrderItem()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };
                order.OrderItems.Add(orderItems);
            
                _logger.LogInformation("Adding product {ProductId} with quantity {Quantity} to order",
                    orderItems.ProductId, orderItems.Quantity);
            }
            _logger.LogInformation("Order contains {ItemCount} items", order.OrderItems.Count);
            
            var created = await _orderRepository.CreateAsync(order);
                    
            _logger.LogInformation("Order created successfully with ID: {OrderId}", created.Id);
            
            await _unitOfWork.CommitAsync();
            
            var response = new OrderResponse
            {
                Id = created.Id,
                CustomerId = created.CustomerId,
                EmployeeId = created.EmployeeId,
                CreatedAt = created.CreatedAt,
                Status = OrderStatus.Pending,
                Items = created.OrderItems.Select(oi=> new OrderItemResponse()
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    TotalPrice = oi.Quantity * oi.Price
                }).ToList()
            };
            return response;
        }
        
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while creating order for Customer ID: {CustomerId}",
                request.CustomerId);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<PaymentResponse> PayOrderAsync(int orderId, decimal amountPaid)
    {
        _logger.LogInformation("Processing payment for Order {OrderId}", orderId);
        
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }

        if (order.Status == OrderStatus.Paid)
        {
            throw new OrderAlreadyPaidException(orderId);
        }
        
        var total = order.OrderItems.Sum(i => i.Price * i.Quantity);
        if (amountPaid < total)
        {
            throw new InsufficientPaymentException(amountPaid, total);
        }

        var change = amountPaid - total;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                
                if (product == null)
                {
                    throw new ProductNotFoundException(item.ProductId);
                }

                if (product.Stock < item.Quantity)
                {
                    throw new InsufficientStockException(item.ProductId, product.Stock, item.Quantity);
                }
                
                product.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order);

            await _unitOfWork.CommitAsync();

            return new PaymentResponse()
            {
                Total = total,
                Paid = amountPaid,
                Change = change
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        
        if (order == null)
        {
            throw new OrderNotFoundException(orderId);
        }

        if (order.Status == OrderStatus.Paid)
        {
            throw new OrderAlreadyPaidException(orderId);
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOrderException($"Order {orderId} is already cancelled");
        }
        
        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOrderException("Only pending orders can be cancelled");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Cancelling Order {OrderId}", orderId);
            
            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<OrderResponse?> GetByIdAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", id);
            throw new OrderNotFoundException(id);
        }

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            EmployeeId = order.EmployeeId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.OrderItems.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.Quantity * i.Price
            }).ToList()
        };
    }
}