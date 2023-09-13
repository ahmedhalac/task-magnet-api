using TaskMagnet.Common.Shared;
using TaskMagnet.Core.Services.Interfaces;
using TaskMagnet.Infrastructure.Database;

namespace TaskMagnet.Core.Services;

public class UserService : IUserService
{
    private readonly TaskMagnetDBContext _dbContext;
    public UserService(TaskMagnetDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Message> RegisterUserAsMessageAsync()
    {
        throw new NotImplementedException();
    }
}
