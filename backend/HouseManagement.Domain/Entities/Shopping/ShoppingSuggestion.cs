using HouseManagement.Domain.Common;

namespace HouseManagement.Domain.Entities;

public class ShoppingSuggestion : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string NameNormalized { get; set; } = string.Empty;
  public int UsageCount { get; set; } // ranking de relevância
  public DateTime LastUsedAt { get; set; }
}