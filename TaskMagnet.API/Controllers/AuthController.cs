using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskMagnet.Common.Dtos;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Core.Services;


namespace TaskMagnet.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    public AuthController(IAuthService authService, UserManager<User> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var message = await _authService.Login(loginDto);
        if(!message.IsValid)
            return Unauthorized(message);

        if(message.Status != Common.Shared.ExceptionCodeEnum.Success)
            return BadRequest(message);
        
        (SessionDto Session, string RefreshToken) authData = ((SessionDto, string))message.Data;

        Response.Cookies.Append("X-Refresh-Token", authData.RefreshToken, new CookieOptions() {HttpOnly = true, SameSite = SameSiteMode.Strict, Secure = false});
        
        return Ok(authData.Session);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        Request.Cookies.TryGetValue("X-Refresh-Token", out var refreshToken);

        var message = await _authService.RefreshToken(refreshToken);
        
        if(!message.IsValid)
            return Unauthorized(message);

        return Ok(message.Data);
    }

    [Authorize, HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        await _authService.LogoutAsync(user);
        return Ok();
    }
}
