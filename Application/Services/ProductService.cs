using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;
    private readonly IOrderRepository _orderRepository;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger, IOrderRepository orderRepository)
    {
        _repository = repository;
        _logger = logger;
        _orderRepository = orderRepository;
    }
    
    public async Task<PagedResponse<ProductResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching products. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new PagedResponse<ProductResponse>();
        }
        
        var product = await _repository.GetAllAsync(pageNumber, pageSize);
        var items = product.Items.Select(p => new ProductResponse()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Stock = p.Stock,
            CategoryId = p.CategoryId
        }).ToList();
        
        _logger.LogInformation("Returned {Count} products out of {Total}", items.Count, product.TotalCount);
        return new PagedResponse<ProductResponse>()
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = product.TotalCount
        };
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            throw new ProductNotFoundException(id);
        }
        
        return new ProductResponse()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
    }

    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);
        
        var product = new Product()
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };
        var createdProduct = await _repository.AddAsync(product);
        
        _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);
        
        return new ProductResponse
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
            Stock = createdProduct.Stock,
            CategoryId = createdProduct.CategoryId
        };
    }

    public async Task<ProductResponse?> UpdateAsync(int id, UpdateProductRequest request)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with {ProductId} not found", id);
            throw new ProductNotFoundException(id);
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;

        var updated = await _repository.UpdateAsync(product);
        
        _logger.LogInformation("Product {ProductId} updated successfully. Name: {Name}, Price: {Price}",
            updated.Id, updated.Name, updated.Price);
        
        return new ProductResponse()
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price,
            Stock = updated.Stock,
            CategoryId = updated.CategoryId
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var isUsed = await _orderRepository.AnyOrderContainsProductAsync(id);
        
        if (isUsed)
        {
            throw new ProductInUseException(id);
        }
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            throw new ProductNotFoundException(id);
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);

        return true;
    }
}