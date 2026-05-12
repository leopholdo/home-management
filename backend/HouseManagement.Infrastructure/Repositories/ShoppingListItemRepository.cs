using HouseManagement.Application.Interfaces.Repositories;
using HouseManagement.Domain.Entities;
using HouseManagement.Infrastructure.Context;
using HouseManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HouseManagement.Infrastructure.Repositories;

public class ShoppingListItemRepository : IShoppingListItemRepository
{
    private readonly HouseManagementDbContext _context;

    public ShoppingListItemRepository(HouseManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingListItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingListItems
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ShoppingListItem>> GetByShoppingListIdAsync(Guid shoppingListId, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingListItems
            .Where(i => i.ShoppingListId == shoppingListId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingListItem> AddAsync(ShoppingListItem item, CancellationToken cancellationToken = default)
    {
        _context.ShoppingListItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(ShoppingListItem item, CancellationToken cancellationToken = default)
    {
        _context.ShoppingListItems.Update(item);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ShoppingListItem item, CancellationToken cancellationToken = default)
    {
        _context.ShoppingListItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
    }
}