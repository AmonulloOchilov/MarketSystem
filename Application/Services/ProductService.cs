using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repository, ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<List<ProductResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching products. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new List<ProductResponse>();
        }
        
        var product = await _repository.GetAllAsync(pageNumber, pageSize);
        var result = product.Select(p => new ProductResponse()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryId = p.CategoryId
        }).ToList();
        
        _logger.LogInformation("Returned {Count} products", result.Count);
        return result;
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            return null;
        }
        
        return new ProductResponse()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
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
            CategoryId = request.CategoryId
        };
        var createdProduct = await _repository.AddAsync(product);
        
        _logger.LogInformation("Product created successfully with ID: {ProductId}", createdProduct.Id);
        
        return new ProductResponse
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
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
            return null;
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;

        var updated = await _repository.UpdateAsync(product);
        
        _logger.LogInformation("Product {ProductId} updated successfully. Name: {Name}, Price: {Price}",
            updated.Id, updated.Name, updated.Price);
        
        return new ProductResponse()
        {
            Id = updated.Id,
            Name = updated.Name,
            Price = updated.Price,
            CategoryId = updated.CategoryId
        };
    }

    public async Task<ProductResponse?> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", id);
            return null;
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
        
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CategoryId = product.CategoryId
        };
    }
}