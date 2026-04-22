using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<ActionResult<OrderResponse>> GetAllAsync()
    {
        return Ok(await _service.GetAllAsync());
    }
    
    [HttpGet("id")]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateOrderRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }
}