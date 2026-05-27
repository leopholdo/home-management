using Microsoft.Extensions.DependencyInjection;
using HouseManagement.Application.Services;
using HouseManagement.Application.Interfaces.Repositories;
using HouseManagement.Application.Interfaces.Services;
using HouseManagement.Infrastructure.Repositories;
using HouseManagement.Application.Profiles;

namespace HouseManagement.Infrastructure.DependencyInjection;

public static class ApplicationDI
{
    /// <summary>
    /// Registers all ShoppingList-related services, repositories, and mappings.
    /// Call this from Program.cs: builder.Services.AddShoppingListModule();
    /// </summary>
    public static IServiceCollection Inject(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
        services.AddScoped<IShoppingListItemRepository, ShoppingListItemRepository>();
        services.AddScoped<IShoppingSuggestionRepository, ShoppingSuggestionRepository>();

        // Services
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IShoppingSuggestionService, ShoppingSuggestionService>();

        // AutoMapper profile
        services.AddAutoMapper(cfg => cfg.AddProfile<AutomapperProfiles>());

        return services;
    }
}