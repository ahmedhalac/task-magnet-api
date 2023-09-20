using Microsoft.EntityFrameworkCore;
using TaskMagnet.API.Constants;
using TaskMagnet.API.Extensions;
using TaskMagnet.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(setupAction => 
{
    setupAction.SwaggerDoc(
        "TaskMagnetOpenAPISpecification",
        new Microsoft.OpenApi.Models.OpenApiInfo()
        {
            Title = "TaskMagnet API",
            Version = "1"
        });
});

builder.Services.AddDbContext<TaskMagnetDBContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString(Config.CONNECTION_STRING)));

// Dependency injection. 
builder.Services.AddDependecyInjection(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction => 
    {
        setupAction.SwaggerEndpoint(
            "/swagger/TaskMagnetOpenAPISpecification/swagger.json",
            "TaskMagnet API");
    });
}

app.UseCors(policy => policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200"));

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
