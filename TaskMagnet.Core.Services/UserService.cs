using System.Data.Common;
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
    public UserService(TaskMagnetDBContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    async Task<Message> IUserService.RegisterNewUserAsMessageAsync(UserRegisterDto userRegisterDto)
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
                IsValid = true
            };
        }
        else 
        {
            var errorMessage = result.Errors.Select(error => error.Description);
            return new Message
            {
                Status = ExceptionCodeEnum.BadRequest,
                Info = string.Join(", ", errorMessage),
                IsValid = false,
            };
        }
        
    }
}
