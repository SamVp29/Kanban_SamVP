using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;

    public AuthController(IAuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var response = await _authUseCase.LoginAsync(request);
        if (response == null) return Unauthorized("Credenciales inválidas");

        return Ok(response);
    }
}
