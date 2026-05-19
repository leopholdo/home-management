using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Infrastructure.Persistence.Configurations;

public class SuggestionConfiguration : IEntityTypeConfiguration<ShoppingSuggestion>
{
  public void Configure(EntityTypeBuilder<ShoppingSuggestion> builder)
  {
    builder.ToTable("ShoppingSuggestions");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
      .HasMaxLength(200)
      .IsRequired();

    builder.Property(x => x.NameNormalized)
      .HasMaxLength(200)
      .IsRequired();

    builder.HasIndex(x => x.Name).IsUnique();

    builder.HasIndex(x => x.NameNormalized)
      .HasMethod("GIN")
      .HasOperators("gin_trgm_ops");

    builder.HasIndex(x => new { x.UsageCount, x.LastUsedAt });
  }
}