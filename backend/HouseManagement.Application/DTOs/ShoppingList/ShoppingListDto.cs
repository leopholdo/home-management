namespace HouseManagement.Application.DTOs.ShoppingList;

public class ShoppingListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
    public ICollection<ShoppingListItemDto> Items { get; set; } = new List<ShoppingListItemDto>();
    public DateTime CreatedAt { get; set; }
}

public class ShoppingListItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsCompleted { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Guid ShoppingListId { get; set; }
}

public record ShoppingListSummaryDto(
    Guid Id,
    string Name,
    int TotalItems,
    int CompletedItems
);