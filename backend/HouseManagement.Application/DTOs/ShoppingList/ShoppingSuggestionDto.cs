public record ShoppingSuggestionDto(
  Guid Id,
  string Name,
  int UsageCount
);

public record AddSuggestionRequest(
  string Name
);