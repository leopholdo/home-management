using HouseManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
namespace HouseManagement.API;

public static class Startup
{
  public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
  {
    // Add services to the container.
    services.AddControllers();

    // Swagger
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    // Database context
    services.ConfigureDbContext(configuration);
  }

  public static WebApplication Configure(this WebApplicationBuilder builder)
  {
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    // TO-DO: 
    // app.ConfigureCORS();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    return app;
  }

  private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<HouseManagementDbContext>(options =>
    {
      options.UseNpgsql(connectionString);
    });
  }
}