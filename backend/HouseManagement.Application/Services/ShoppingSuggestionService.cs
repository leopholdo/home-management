using HouseManagement.Domain.Entities;

public class ShoppingSuggestionService : IShoppingSuggestionService
{
  private readonly IShoppingSuggestionRepository _repository;

  public ShoppingSuggestionService(IShoppingSuggestionRepository repository)
      => _repository = repository;

  public async Task<IEnumerable<ShoppingSuggestionDto>> GetTopAsync(int limit = 10, CancellationToken ct = default)
  {
    var items = await _repository.GetTopAsync(limit, ct);
    return items.Select(Map);
  }

  public async Task<IEnumerable<ShoppingSuggestionDto>> SearchAsync(
      string term, int limit = 10, CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(term))
      return await GetTopAsync(limit, ct);

    var termNormalized = TextNormalizer.Normalize(term);
    var items = await _repository.SearchAsync(termNormalized, limit, ct);
    return items.Select(Map);
  }

  public async Task AddSuggestionAsync(string name, CancellationToken ct = default)
  {
    var nameNormalized = TextNormalizer.Normalize(name);
    var existing = await _repository.GetByNameNormalizedAsync(nameNormalized, ct);

    // prevents duplicate suggestions
    if (existing is not null)
    {
      existing.UsageCount++;
      existing.LastUsedAt = DateTime.UtcNow;
    }
    else
    {
      await _repository.AddAsync(new ShoppingSuggestion
      {
        Id = Guid.NewGuid(),
        Name = name.Trim(),
        NameNormalized = nameNormalized,
        UsageCount = 1,
        LastUsedAt = DateTime.UtcNow
      }, ct);
    }

    await _repository.SaveChangesAsync(ct);
  }

  private static ShoppingSuggestionDto Map(ShoppingSuggestion s)
      => new(s.Id, s.Name, s.UsageCount);
}