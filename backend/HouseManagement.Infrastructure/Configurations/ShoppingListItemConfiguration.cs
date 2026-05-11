using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Infrastructure.Persistence.Configurations;

public class ShoppingListItemConfiguration : IEntityTypeConfiguration<ShoppingListItem>
{
  public void Configure(EntityTypeBuilder<ShoppingListItem> builder)
  {
    builder.ToTable("ShoppingListItem");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
        .HasMaxLength(100)
        .IsRequired();

    builder.Property(x => x.Notes)
        .HasMaxLength(250)
        .IsRequired(false);

    builder.Property(x => x.Quantity)
      .IsRequired();

    builder.Property(x => x.IsCompleted)
        .IsRequired();

    builder.Property(x => x.ShoppingListId)
        .IsRequired();
  }
}