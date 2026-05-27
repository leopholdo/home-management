using HouseManagement.Domain.Entities;
using HouseManagement.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

public class ShoppingSuggestionRepository : IShoppingSuggestionRepository
{
  private readonly HouseManagementDbContext _context;

  public ShoppingSuggestionRepository(HouseManagementDbContext context)
    => _context = context;

  public async Task<IEnumerable<ShoppingSuggestion>> GetTopAsync(int limit, CancellationToken ct)
    => await _context.ShoppingSuggestions
      .OrderByDescending(x => x.UsageCount)
      .ThenByDescending(x => x.LastUsedAt)
      .Take(limit)
      .AsNoTracking()
      .ToListAsync(ct);

  public async Task<IEnumerable<ShoppingSuggestion>> SearchAsync(
      string termNormalized, int limit, CancellationToken ct)
  {
    IQueryable<ShoppingSuggestion> query = _context.ShoppingSuggestions.AsNoTracking();

    if (string.IsNullOrWhiteSpace(termNormalized))
    {
      return await query
        .OrderByDescending(x => x.UsageCount)
        .ThenBy(x => x.Name)
        .Take(limit)
        .ToListAsync(ct);
    }

    return await query
      .Where(x =>
          x.NameNormalized.StartsWith(termNormalized)   // prefix — B-tree, mais rápido
          || x.NameNormalized.Contains(termNormalized)  // substring — GIN
          || HouseManagementDbContext.TrigramSimilarity(x.NameNormalized, termNormalized) > 0.3) // fuzzy — GIN
      .OrderByDescending(x => x.NameNormalized.StartsWith(termNormalized) ? 2  // prefix wins
          : x.NameNormalized.Contains(termNormalized) ? 1                       // substring second
          : 0)                                                                   // fuzzy last
      .ThenByDescending(x => x.UsageCount)
      .ThenBy(x => x.Name)
      .ThenByDescending(x => x.LastUsedAt)
      .Take(limit)
      .AsNoTracking()
      .ToListAsync(ct);
  }

  public async Task<ShoppingSuggestion?> GetByNameNormalizedAsync(string nameNormalized, CancellationToken ct)
    => await _context.ShoppingSuggestions
      .FirstOrDefaultAsync(x => x.NameNormalized == nameNormalized, ct);

  public async Task AddAsync(ShoppingSuggestion suggestion, CancellationToken ct)
    => await _context.ShoppingSuggestions.AddAsync(suggestion, ct);

  public Task SaveChangesAsync(CancellationToken ct)
    => _context.SaveChangesAsync(ct);
}