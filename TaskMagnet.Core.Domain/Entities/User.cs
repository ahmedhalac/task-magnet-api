using Microsoft.AspNetCore.Identity;

namespace TaskMagnet.Core.Domain.Entities;

public class User : IdentityUser<long>
{
    public string FirstName { get; set; }
    public string LastName { get; set;}
    public string Country { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpireDate { get; set; }
}
