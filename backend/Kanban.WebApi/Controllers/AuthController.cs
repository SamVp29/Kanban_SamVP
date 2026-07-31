using Kanban.Application.DTOs;
using Kanban.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null) return Unauthorized("Credenciales inválidas");

        return Ok(response);
    }
}
