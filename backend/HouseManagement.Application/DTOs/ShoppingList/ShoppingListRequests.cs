namespace HouseManagement.Application.DTOs.ShoppingList;

public class CreateShoppingListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateShoppingListItemRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class UpdateShoppingListRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsCompleted { get; set; }
}

public class UpdateShoppingListItemRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool IsCompleted { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class AddShoppingListItemBatchRequest
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class UpdateShoppingListItemBatchRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

public class DeleteShoppingListItemBatchRequest
{
    public Guid Id { get; set; }
}

public class UpsertBatchShoppingListItemsRequest
{
    public List<AddShoppingListItemBatchRequest> ItemsToAdd { get; set; } = new();
    public List<UpdateShoppingListItemBatchRequest> ItemsToUpdate { get; set; } = new();
    public List<DeleteShoppingListItemBatchRequest> ItemsToRemove { get; set; } = new();
}