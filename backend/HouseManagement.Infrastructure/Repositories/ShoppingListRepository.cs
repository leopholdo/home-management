using HouseManagement.Application.DTOs.ShoppingList;
using HouseManagement.Application.Interfaces.Repositories;
using HouseManagement.Domain.Entities;
using HouseManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace HouseManagement.Infrastructure.Repositories;

public class ShoppingListRepository : IShoppingListRepository
{
  private readonly HouseManagementDbContext _context;

  public ShoppingListRepository(HouseManagementDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<ShoppingListSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await _context.ShoppingLists
      .AsNoTracking()
      .Select(list => new ShoppingListSummaryDto(
        list.Id,
        list.Name,
        list.Items.Count,
        list.Items.Count(item => item.IsCompleted)
      ))
      .ToListAsync(cancellationToken);
  }

  public async Task<ShoppingList?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _context.ShoppingLists
      .Include(sl => sl.Items)
      .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);
  }

  public async Task<ShoppingList> AddAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default)
  {
    _context.ShoppingLists.Add(shoppingList);
    await _context.SaveChangesAsync(cancellationToken);
    return shoppingList;
  }

  public async Task UpdateAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default)
  {
    _context.ShoppingLists.Update(shoppingList);
    await _context.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteAsync(ShoppingList shoppingList, CancellationToken cancellationToken = default)
  {
    _context.ShoppingLists.Remove(shoppingList);
    await _context.SaveChangesAsync(cancellationToken);
  }
}