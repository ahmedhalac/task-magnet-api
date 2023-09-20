using TaskMagnet.Common.Dtos;
using TaskMagnet.Common.Shared;
using TaskMagnet.Core.Domain.Entities;

namespace TaskMagnet.Core.Services;

public interface IAuthService
{
    Task<Message> Login(LoginDto loginDto);
    Task<Message> RefreshToken(string token);
    Task<Message> LogoutAsycn(User user);
}
