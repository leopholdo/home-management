using HouseManagement.Domain.Common;

namespace HouseManagement.Domain.Entities;
public class ShoppingListItem : BaseEntity
{
  public string Name { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public bool IsCompleted { get; set; }
  public string Notes { get; set; } = string.Empty;
  public Guid ShoppingListId { get; private set; }
  public ShoppingList ShoppingList { get; private set; } = null!;
}