using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<EmployeeResponse>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("id")]
    public async Task<ActionResult<EmployeeResponse>> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateEmployeeRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        return Ok(await _service.UpdateAsync(id, request));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
}