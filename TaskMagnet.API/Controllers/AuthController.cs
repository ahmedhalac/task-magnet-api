using Microsoft.AspNetCore.Mvc;
using TaskMagnet.Common.Dtos;
using TaskMagnet.Core.Services;


namespace TaskMagnet.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var message = await _authService.Login(loginDto);
        if(!message.IsValid)
            return Unauthorized(message);

        if(message.Status != Common.Shared.ExceptionCodeEnum.Success)
            return BadRequest(message);

        return Ok(message.Data);
    }
}
