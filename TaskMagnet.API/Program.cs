using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskMagnet.API.Constants;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Core.Services;
using TaskMagnet.Core.Services.Interfaces;
using TaskMagnet.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskMagnetDBContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString(Config.CONNECTION_STRING)));

builder.Services.AddIdentity<User, IdentityRole<long>>(options => {
    options.Password.RequiredLength = 5;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<TaskMagnetDBContext>();

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
