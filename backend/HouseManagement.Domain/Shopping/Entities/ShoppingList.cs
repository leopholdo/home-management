using HouseManagement.Domain.Common;

namespace HouseManagement.Domain.Entities;

public class ShoppingList : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public string? Notes { get; set; }
  public bool IsCompleted { get; set; }
  public ICollection<ShoppingListItem> Items { get; private set; } = new HashSet<ShoppingListItem>();
}