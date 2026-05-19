public interface IShoppingSuggestionService
{
  Task<IEnumerable<ShoppingSuggestionDto>> GetTopAsync(int limit = 10, CancellationToken ct = default);
  Task<IEnumerable<ShoppingSuggestionDto>> SearchAsync(string term, int limit = 10, CancellationToken ct = default);
  Task RecordUsageAsync(string name, CancellationToken ct = default);
}