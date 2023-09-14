using Microsoft.AspNetCore.Mvc;
using TaskMagnet.Core.Services.Interfaces;
using TaskMagnet.Common.Dtos.Users;
using Microsoft.AspNetCore.Authorization;

namespace TaskMagnet.API;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUserAsync(UserRegisterDto userRegisterDto) 
    {
        var message = await _userService.RegisterNewUserAsMessageAsync(userRegisterDto);
        if(!message.IsValid)
            return BadRequest();

        return Ok(message);
    }
}
