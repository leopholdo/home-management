using HouseManagement.Domain.Entities;

namespace HouseManagement.Application.Interfaces.Repositories;

public interface IShoppingListItemRepository
{
    Task<ShoppingListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default);
    Task<ShoppingListItem> AddAsync(ShoppingListItem item, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShoppingListItem item, CancellationToken cancellationToken = default);
    Task DeleteAsync(ShoppingListItem item, CancellationToken cancellationToken = default);
}