using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TaskMagnet.Common.Dtos.Users;
using TaskMagnet.Common.Shared;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Core.Services.Interfaces;
using TaskMagnet.Infrastructure.Database;

namespace TaskMagnet.Core.Services;

public class UserService : IUserService
{
    private readonly TaskMagnetDBContext _dbContext;
    private UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserService(TaskMagnetDBContext dbContext, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Message> RegisterNewUserAsMessageAsync(UserRegisterDto userRegisterDto)
    {
        var newUser = new User
        {
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
            UserName = userRegisterDto.UserName,
            Email = userRegisterDto.Email,
            Country = userRegisterDto.Country
        };

        var result = await _userManager.CreateAsync(newUser, userRegisterDto.Password);

        if(result.Succeeded)
        {
            return new Message
            {
                Status = ExceptionCodeEnum.Success,
                IsValid = false
            };
        }
        else 
        {
            var errorMessage = result.Errors.Select(error => error.Description);
            return new Message
            {
                Status = ExceptionCodeEnum.BadRequest,
                Info = string.Join(", ", errorMessage),
                IsValid = true,
            };
        }
        
    }

    public async Task<Message> GetLoggedInUserAsMessageAsync()
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

        if(user == null)
        {
            return new Message
            {
                Status = ExceptionCodeEnum.Unauthorized,
                IsValid = false
            };
        }

        return new Message
        {
            Data  = user,
            IsValid = true
        };
    }
}
