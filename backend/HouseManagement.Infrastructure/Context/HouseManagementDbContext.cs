using Microsoft.EntityFrameworkCore;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Infrastructure.Context;

public class HouseManagementDbContext : DbContext
{
  public HouseManagementDbContext(DbContextOptions<HouseManagementDbContext> options)
      : base(options)
  {
  }

  public DbSet<ShoppingList> ShoppingLists => Set<ShoppingList>();
  public DbSet<ShoppingListItem> ShoppingListItems => Set<ShoppingListItem>();

  // TODO: Feature ainda por implementar.
  // public DbSet<ShoppingSuggestion> Suggestions => Set<ShoppingSuggestion>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(HouseManagementDbContext).Assembly);

    ConfigureGlobalConventions(modelBuilder);
  }

  private void ConfigureGlobalConventions(ModelBuilder modelBuilder)
  {
    // Padronização de DateTime sem timestamp
    foreach (var property in modelBuilder.Model.GetEntityTypes()
        .SelectMany(t => t.GetProperties())
        .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
    {
      property.SetColumnType("timestamp with time zone");
    }
  }
}