using Kanban.Application.DTOs;
using Kanban.Application.Ports.In;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanban.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IAuthUseCase _authUseCase;

    public UsuariosController(IAuthUseCase authUseCase)
    {
        _authUseCase = authUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetAll()
    {
        var users = await _authUseCase.GetAllUsersAsync();
        return Ok(users);
    }
}
