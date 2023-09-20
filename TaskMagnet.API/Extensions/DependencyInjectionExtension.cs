using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Core.Services;
using TaskMagnet.Core.Services.Interfaces;
using TaskMagnet.Infrastructure.Common.Configurations;
using TaskMagnet.Infrastructure.Database;

namespace TaskMagnet.API.Extensions;

public static class DependencyInjectionExtension
{
    public static void AddDependecyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfig = configuration.GetSection("jwt").Get<JwtConfiguration>() ?? new JwtConfiguration();
        services.AddSingleton(jwtConfig);

        // Add identity
        services.AddIdentity<User, IdentityRole<long>>(options => {
            options.Password.RequiredLength = 5;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }).AddEntityFrameworkStores<TaskMagnetDBContext>();

        // Jwt configuration
        services
            .AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        // Services DI.
        services.AddScoped<IUserService, UserService>();
    }
}
