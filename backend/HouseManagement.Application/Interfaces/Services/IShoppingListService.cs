using HouseManagement.Application.DTOs.ShoppingList;

namespace HouseManagement.Application.Interfaces.Services;

public interface IShoppingListService
{
    Task<IEnumerable<ShoppingListSummaryDto>> GetAllAsync(bool isDeleted, CancellationToken cancellationToken = default);
    Task<ShoppingListDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShoppingListDto> CreateAsync(CreateShoppingListRequest request, CancellationToken cancellationToken = default);
    Task<ShoppingListDto> UpdateAsync(Guid id, UpdateShoppingListRequest request, CancellationToken cancellationToken = default);
    Task ToggleDeletedAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Item operations
    Task<ShoppingListItemDto> AddItemAsync(Guid shoppingListId, CreateShoppingListItemRequest request, CancellationToken cancellationToken = default);
    Task<ShoppingListItemDto> UpdateItemAsync(Guid shoppingListId, Guid itemId, UpdateShoppingListItemRequest request, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(Guid shoppingListId, Guid itemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShoppingListItemDto>> UpsertBatchItemsAsync(Guid shoppingListId, UpsertBatchShoppingListItemsRequest request, CancellationToken cancellationToken = default);
}