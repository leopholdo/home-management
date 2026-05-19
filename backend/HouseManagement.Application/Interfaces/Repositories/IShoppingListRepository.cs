using HouseManagement.Application.DTOs.ShoppingList;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Application.Interfaces.Repositories;

public interface IShoppingListRepository
{
    Task<IEnumerable<ShoppingListSummaryDto>> GetAllAsync(bool isDeleted, CancellationToken cancellationToken = default);
    Task<ShoppingList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ShoppingList> AddAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default);
    Task UpdateAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default);
    Task ToggleDeletedAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default);
    Task DeleteAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default);
}