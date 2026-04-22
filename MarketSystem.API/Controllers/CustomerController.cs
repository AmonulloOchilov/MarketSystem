using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerResponse>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("id")]
    public async Task<ActionResult<CustomerResponse>> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateCustomerRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        return Ok(await _service.UpdateAsync(id, request));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        return Ok(await _service.DeleteAsync(id));
    }
}