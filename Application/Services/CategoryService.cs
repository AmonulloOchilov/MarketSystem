using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(c => new CategoryResponse()
        {
            Id = c.Id,
            Name = c.Name
        }).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name
        };
        var createdCategory = await _repository.AddAsync(category);
        return new CategoryResponse()
        {
            Id = createdCategory.Id,
            Name = createdCategory.Name
        };
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        category.Name = request.Name;
        await _repository.UpdateAsync(category);
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryResponse?> DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            return null;
        }

        await _repository.DeleteAsync(id);
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}