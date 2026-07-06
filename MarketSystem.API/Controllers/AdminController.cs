using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Features.Admins.Commands.CreateAdmin;
using Application.Features.Admins.Commands.DeleteAdmin;
using Application.Features.Admins.Commands.UpdateAdmin;
using Application.Features.Admins.Queries.GetAdminById;
using Application.Features.Admins.Queries.GetAllAdmins;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<AdminResponse>> GetAllAsync([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllAdminsQuery(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AdminResponse>> GetByIdAsync(int id)
    {
        var result = await _mediator.Send(new GetAdminByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateAdminRequest request)
    {
        var result = await _mediator.Send(new CreateAdminCommand(request));
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateAdminRequest request)
    {
        var result = await _mediator.Send(new UpdateAdminCommand(id, request));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _mediator.Send(new DeleteAdminCommand(id));
        return NoContent();
    }
}