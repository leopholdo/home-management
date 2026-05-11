using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Infrastructure.Persistence.Configurations;

public class ShoppingListConfiguration : IEntityTypeConfiguration<ShoppingList>
{
  public void Configure(EntityTypeBuilder<ShoppingList> builder)
  {
    builder.ToTable("ShoppingList");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(x => x.Notes)
        .HasMaxLength(250)
        .IsRequired(false);

    builder.Property(x => x.IsCompleted)
        .IsRequired();

    builder.HasMany(x => x.Items)
        .WithOne(x => x.ShoppingList)
        .HasForeignKey(x => x.ShoppingListId)
        .OnDelete(DeleteBehavior.Cascade);
  }
}