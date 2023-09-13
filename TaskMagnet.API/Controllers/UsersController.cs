using Microsoft.AspNetCore.Mvc;
using TaskMagnet.Core.Services.Interfaces;

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

    [HttpPost]
    public async Task<IActionResult> RegisterUserAsync() 
    {
        var message = await _userService.RegisterUserAsMessageAsync();
        if(!message.IsValid)
            return BadRequest();
        return Ok(message);
    }
}
