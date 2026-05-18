using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Features.Orders.Commands.CancelOrder;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.PayOrder;
using Application.Features.Orders.Queries.GetAllOrders;
using Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Customer,Employee")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<OrderResponse>> GetAllAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(pageNumber, pageSize));
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(int id)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateOrderRequest request)
    {
        var result = await _mediator.Send(new CreateOrderCommand(request));
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAsync(int id)
    {
        await _mediator.Send(new CancelOrderCommand(id));
        return NoContent();
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayAsync(int id, PayOrderRequest request)
    {
        var result = await _mediator.Send(new PayOrderCommand(id, request.AmountPaid));
        return Ok(result);
    }
}