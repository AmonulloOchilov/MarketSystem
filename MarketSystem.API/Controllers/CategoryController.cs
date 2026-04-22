using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CategoryResponse>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("id")]
    public async Task<ActionResult<CategoryResponse>> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCategoryRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        return Ok(await _service.UpdateAsync(id, request));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}