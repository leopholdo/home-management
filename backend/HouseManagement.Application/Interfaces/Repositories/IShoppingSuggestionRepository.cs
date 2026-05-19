using HouseManagement.Domain.Entities;

public interface IShoppingSuggestionRepository
{
  Task<IEnumerable<ShoppingSuggestion>> GetTopAsync(int limit, CancellationToken ct);
  Task<IEnumerable<ShoppingSuggestion>> SearchAsync(string termNormalized, int limit, CancellationToken ct);
  Task<ShoppingSuggestion?> GetByNameNormalizedAsync(string nameNormalized, CancellationToken ct);
  Task AddAsync(ShoppingSuggestion suggestion, CancellationToken ct);
  Task SaveChangesAsync(CancellationToken ct);
}