using TaskMagnet.Common.Shared;

namespace TaskMagnet.Core.Services.Interfaces;

public interface IUserService
{
    Task<Message> RegisterUserAsMessageAsync();
}
