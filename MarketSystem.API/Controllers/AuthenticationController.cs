using Application.DTOs.Request;
using Application.Features.Authentications.Commands.Login;
using Application.Features.Authentications.Commands.Refresh;
using Application.Features.Authentications.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(new LoginCommand(request));
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _mediator.Send(new RegisterCommand(request));
        return Ok("User created");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request));
        return Ok(result);
    }
}