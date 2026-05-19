using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Infrastructure.Persistence.Configurations;

public class SuggestionConfiguration : IEntityTypeConfiguration<ShoppingSuggestion>
{
  public void Configure(EntityTypeBuilder<ShoppingSuggestion> builder)
  {
    builder.ToTable("Suggestion");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Name)
        .HasMaxLength(100)
        .IsRequired();
  }
}