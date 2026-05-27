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
  public DbSet<ShoppingSuggestion> ShoppingSuggestions => Set<ShoppingSuggestion>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.ApplyConfigurationsFromAssembly(typeof(HouseManagementDbContext).Assembly);

    modelBuilder.HasDbFunction(
    typeof(HouseManagementDbContext)
        .GetMethod(nameof(TrigramSimilarity))!)
    .HasName("similarity");

    ConfigureGlobalConventions(modelBuilder);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);

    optionsBuilder.UseSeeding((context, _) =>
    {
      // Seed Shopping Suggestions
      if (context.Set<ShoppingSuggestion>().Any()) return;
      context.Set<ShoppingSuggestion>().AddRange(ShoppingSuggestionSeed.Get());

      context.SaveChanges();
    });
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

  [DbFunction("similarity", IsNullable = false)]
  public static double TrigramSimilarity(string a, string b)
    => throw new NotSupportedException("EF Core translation only.");
}