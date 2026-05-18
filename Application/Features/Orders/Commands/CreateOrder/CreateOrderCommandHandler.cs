using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository,
        IEmployeeRepository employeeRepository, IProductRepository productRepository,
        ILogger<CreateOrderCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _employeeRepository = employeeRepository;
        _productRepository = productRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.Request.Items == null || request.Request.Items.Count <= 0)
        {
            throw new InvalidOrderException("Order must contain at least one item");
        }
        
        var hasDuplicates = request.Request.Items
            .GroupBy(x => x.ProductId)
            .Any(g => g.Count() > 1);
        
        if (hasDuplicates)
        {
            throw new DuplicateProductException();
        }

        var customer = await _customerRepository.GetByIdAsync(request.Request.CustomerId);
        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.Request.CustomerId);
            throw new CustomerNotFoundException(request.Request.CustomerId);
        }

        var employee = await _employeeRepository.GetByIdAsync(request.Request.EmployeeId);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.Request.EmployeeId);
            throw new EmployeeNotFoundException(request.Request.EmployeeId);
        }
        
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            _logger.LogInformation("Creating order using Customer ID: {CustomerId}", request.Request.CustomerId);
            var order = new Order
            {
                CustomerId = request.Request.CustomerId,
                EmployeeId = request.Request.EmployeeId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };
                    
            foreach (var item in request.Request.Items)
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
                request.Request.CustomerId);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}