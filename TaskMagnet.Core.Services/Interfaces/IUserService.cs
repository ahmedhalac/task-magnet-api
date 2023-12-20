using TaskMagnet.Common.Dtos.Users;
using TaskMagnet.Common.Shared;

namespace TaskMagnet.Core.Services.Interfaces;

public interface IUserService
{
    Task<Message> RegisterNewUserAsMessageAsync(UserRegisterDto userRegisterDto);
    Task<Message> GetLoggedInUserAsMessageAsync();
}
