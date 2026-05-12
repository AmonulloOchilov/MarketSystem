using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Customer,Employee")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<OrderResponse>> GetAllAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Ok(await _service.GetAllAsync(pageNumber, pageSize));
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateOrderRequest request)
    {
        return Ok(await _service.CreateAsync(request));
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _service.CancelOrderAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayAsync(int id, PayOrderRequest request)
    {
        var result = await _service.PayOrderAsync(id, request.AmountPaid);
        return Ok(result);
    }
}