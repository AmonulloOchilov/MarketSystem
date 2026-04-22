using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryService> _logger;
    public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<PagedResponse<CategoryResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching categories. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new PagedResponse<CategoryResponse>();
        }

        var categories = await _repository.GetAllAsync(pageNumber,pageSize);
        
        var items = categories.Items.Select(c => new CategoryResponse()
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();

        _logger.LogInformation("Returned {Count} categories out of {Total}", items.Count, categories.TotalCount);
        
        return new PagedResponse<CategoryResponse>()
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = categories.TotalCount
        };
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", id);
            throw new CategoryNotFoundException(id);
        }
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        _logger.LogInformation("Creating category: {CategoryName}", request.Name);
        
        var category = new Category
        {
            Name = request.Name
        };
        var createdCategory = await _repository.AddAsync(category);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", createdCategory.Id);
        
        return new CategoryResponse()
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name
        };
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", id);
        
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category with {CategoryId} not found", id);
            throw new CategoryNotFoundException(id);
        }

        category.Name = request.Name;
        await _repository.UpdateAsync(category);
        
        _logger.LogInformation("Category {CategoryId} updated successfully. Name: {CategoryName}", id, category.Name);
        
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", id);
            throw new CategoryNotFoundException(id);
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);

        return true;
    }
}