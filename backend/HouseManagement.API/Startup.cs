using HouseManagement.Infrastructure.DependencyInjection;
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

    // Database and Dependency Injection
    services.ConfigureDbContext(configuration);
    services.Inject();
  }

  public static WebApplication Configure(this WebApplicationBuilder builder)
  {
    var app = builder.Build();

    // TO-DO: Descomentar linhas abaixo para habilitar Swagger apenas em ambiente de desenvolvimento
    // if (app.Environment.IsDevelopment())
    // {
    app.UseSwagger();
    app.UseSwaggerUI();
    // }

    app.ConfigureCors();
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

  private static void ConfigureCors(this WebApplication app)
  {
    var origins = app.Configuration.GetSection("AllowedOrigins").Get<string[]>();

    app.UseCors(policy =>
    {
      policy.WithOrigins(origins!);
      policy.AllowAnyMethod();
      policy.AllowAnyHeader();
    });
  }
}