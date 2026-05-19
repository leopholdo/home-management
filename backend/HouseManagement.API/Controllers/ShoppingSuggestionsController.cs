using Microsoft.AspNetCore.Mvc;

namespace HouseManagement.API.Controllers;

[ApiController]
[Route("api/shopping-suggestions")]
public class ShoppingSuggestionsController : ControllerBase
{
  private readonly IShoppingSuggestionService _service;

  public ShoppingSuggestionsController(IShoppingSuggestionService service) => _service = service;

  /// <summary>
  /// Busca sugestões de compras. Sem term = top por uso. Com term = busca com similaridade.
  /// </summary>
  /// <param name="term">O termo de busca.</param>
  /// <param name="limit">O limite de resultados.</param>
  /// <returns>Retorna as sugestões de compras.</returns>
  // GET api/shopping-suggestions
  [HttpGet]
  public async Task<IActionResult> Get(
      [FromQuery] string? term,
      [FromQuery] int limit = 10,
      CancellationToken ct = default)
  {
    var result = string.IsNullOrWhiteSpace(term)
        ? await _service.GetTopAsync(limit, ct)
        : await _service.SearchAsync(term, limit, ct);

    return Ok(result);
  }

  /// <summary>
  /// Registra uso de um produto (incrementa ranking). Chamado quando o usuário confirma a seleção.
  /// </summary>
  /// <param name="request">DTO com o nome do produto</param>
  /// <returns>Retorna um resultado indicando o sucesso ou falha da operação.</returns>
  // POST api/shopping-suggestions/usage
  [HttpPost("usage")]
  public async Task<IActionResult> RecordUsage(
      [FromBody] RecordUsageRequest request,
      CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
      return BadRequest("Name is required.");

    await _service.RecordUsageAsync(request.Name, ct);
    return NoContent();
  }
}