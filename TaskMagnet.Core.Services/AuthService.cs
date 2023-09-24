using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskMagnet.Common.Dtos;
using TaskMagnet.Common.Shared;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Infrastructure.Common.Configurations;
using TaskMagnet.Infrastructure.Database;

namespace TaskMagnet.Core.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private JwtConfiguration _jwtConfiguration { get; set; }
    private TaskMagnetDBContext _dbContext { get; set; }

    public AuthService(UserManager<User> userManager, JwtConfiguration jwtConfiguration, TaskMagnetDBContext dbContext) 
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
        _dbContext = dbContext;
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

        if(userSignInResult) 
        {
            (string accessToken, long expiresIn) = GenerateJwt(user);
            
            await _dbContext.SaveChangesAsync();
            
            return new Message
            {
                Info = "Success",
                IsValid = true,
                Status = ExceptionCodeEnum.Success,
                Data = new {
                    Token = accessToken,
                    ExpiresIn = expiresIn
                }
            };
        }

        return new Message
        {
            Info = "Forbidden",
            IsValid = false,
            Status = ExceptionCodeEnum.Forbidden
        };
    }

    private (string Token, long ExpiresIn) GenerateJwt(User user)
    {
        var claims = new List<Claim> 
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtConfiguration.ExpirationAccessTokenInMinutes));

        var token = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Issuer,
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), new DateTimeOffset(expires).ToUnixTimeSeconds());
    }
}
