using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAllAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetAllAsync(pageNumber, pageSize));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponse>> GetByIdAsync(int id)
    {
        var result = await _service.GetByIdAsync(id);
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateProductRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _service.DeleteAsync(id);
        
        return Ok(result);
    }
}