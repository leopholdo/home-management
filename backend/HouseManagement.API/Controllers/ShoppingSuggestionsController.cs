using Microsoft.AspNetCore.Mvc;

namespace HouseManagement.API.Controllers;

[ApiController]
[Route("api/ShoppingSuggestions")]
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
  // GET api/ShoppingSuggestions
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
  /// Registra uma nova sugestão de produto. Chamado quando o usuário adiciona uma sugestão manualmente.
  /// </summary>
  /// <param name="request">DTO com o nome da sugestão</param>
  /// <returns>Retorna um resultado indicando o sucesso ou falha da operação.</returns>
  // POST api/ShoppingSuggestions/addSuggestion
  [HttpPost("addSuggestion")]
  public async Task<IActionResult> AddSuggestion(
      [FromBody] AddSuggestionRequest request,
      CancellationToken ct = default)
  {
    if (string.IsNullOrWhiteSpace(request.Name))
      return BadRequest("Name is required.");

    await _service.AddSuggestionAsync(request.Name, ct);
    return NoContent();
  }
}