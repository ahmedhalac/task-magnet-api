using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskMagnet.Common.Dtos;
using TaskMagnet.Common.Shared;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Infrastructure.Common.Configurations;

namespace TaskMagnet.Core.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private JwtConfiguration _jwtConfiguration { get; set; }
    public AuthService(UserManager<User> userManager, JwtConfiguration jwtConfiguration) 
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<Message> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);

        if(user is null) 
        {
            return new Message 
            {
                Info = "Forbidden",
                IsValid = false,
                Status = ExceptionCodeEnum.Forbidden
            };
        }

        var userSignInResult = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        // if(userSignInResult) 
        // {
        //     (string accessToken, long expiresIn) = GenerateJwt(user);
        // }
         return new Message
            {
                Info = "Forbidden",
                IsValid = false,
                Status = ExceptionCodeEnum.Forbidden
            };
    }

    public Task<Message> LogoutAsycn(User user)
    {
        throw new NotImplementedException();
    }

    public Task<Message> RefreshToken(string token)
    {
        throw new NotImplementedException();
    }

    // private (string Token, long ExpiresIn) GenerateJwt(User user)
    // {
    //     var claims = new List<Claim> 
    //     {
    //         new Claim(ClaimTypes.Name, user.UserName),
    //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    //     };

    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret))
    // }
}
