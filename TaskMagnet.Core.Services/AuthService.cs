using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    private SignInManager<User> _signInManager { get; set; }

    public AuthService(UserManager<User> userManager, JwtConfiguration jwtConfiguration, TaskMagnetDBContext dbContext, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
        _dbContext = dbContext;
        _signInManager = signInManager;
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
            var refreshToken = SetRefreshToken(user);
            
            await _dbContext.SaveChangesAsync();

            var session = await GetSession(user.Id, accessToken, expiresIn);
            
            return new Message
            {
                Info = "Success",
                IsValid = true,
                Status = ExceptionCodeEnum.Success,
                Data = (session, refreshToken)
            };
        }

        return new Message
        {
            Info = "Forbidden",
            IsValid = false,
            Status = ExceptionCodeEnum.Forbidden
        };
    }

    private string SetRefreshToken(User user)
    {
        user.RefreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        user.RefreshTokenExpireDate = DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpirationRefreshTokenInMinutes);
        return user.RefreshToken;
    }

    public async Task<Message> RefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new Message
            {
                Info = "Forbidden",
                IsValid = false,
                Status = ExceptionCodeEnum.Forbidden
            };
        }

        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.RefreshToken == refreshToken);

        if (user is null)
        {
            return new Message
            {
                Info = "Forbidden",
                IsValid = false,
                Status = ExceptionCodeEnum.Forbidden
            };
        }
        else if(user.RefreshTokenExpireDate <= DateTime.UtcNow)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpireDate = DateTime.MinValue;
            await _dbContext.SaveChangesAsync();
            return new Message
            {
                Info = "Forbidden",
                IsValid = false,
                Status = ExceptionCodeEnum.Forbidden
            };
        }

        (string accessToken, long expiresIn) = GenerateJwt(user);
        var session = await GetSession(user.Id, accessToken, expiresIn);

        return new Message
        {
            Info = "Success",
            IsValid = true,
            Status = ExceptionCodeEnum.Success,
            Data = session
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

    private async Task<SessionDto> GetSession(long userId, string accessToken, long accessTokenExpiration)
    {
        var websiteUser = await _dbContext.Users
            .Select(x => new
            {
                UserId = x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Country
            })
            .FirstOrDefaultAsync(x => x.UserId == userId);

        var user = await _userManager.FindByIdAsync(userId.ToString());

        var session = new SessionDto
        {
            UserId = userId,
            FirstName = websiteUser?.FirstName,
            LastName = websiteUser?.LastName,
            Email = websiteUser?.Email,
            Token = accessToken,
            TokenExpireDate = accessTokenExpiration
        };

        return session;
    }

    public async Task<Message> LogoutAsync(User user)
    {
        await _signInManager.SignOutAsync();

        user.RefreshToken = null;
        user.RefreshTokenExpireDate = DateTime.MinValue;
        await _dbContext.SaveChangesAsync();

        return new Message
        {
            Info = "Success",
            IsValid = true,
            Status = ExceptionCodeEnum.Success,
        };
    }
}
