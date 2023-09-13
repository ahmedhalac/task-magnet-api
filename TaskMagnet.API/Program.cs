using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskMagnet.API.Constants;
using TaskMagnet.Core.Domain.Entities;
using TaskMagnet.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskMagnetDBContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString(Config.CONNECTION_STRING)));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TaskMagnetDBContext>();

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
