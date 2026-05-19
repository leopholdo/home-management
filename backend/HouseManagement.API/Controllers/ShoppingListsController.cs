using HouseManagement.Application.DTOs.ShoppingList;
using HouseManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace HouseManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    /// <summary>
    /// Busca todas as listas de compras.
    /// </summary>
    /// <returns>Retorna todas as listas de compras.</returns>
    // GET api/shoppinglists
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShoppingListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isDeleted, CancellationToken cancellationToken)
    {
        var lists = await _shoppingListService.GetAllAsync(isDeleted ?? false, cancellationToken);
        return Ok(lists);
    }

    /// <summary>
    /// Busca uma lista de compras pelo ID.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    /// <returns>Retorna a lista de compras correspondente.</returns>
    // GET api/shoppinglists/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var list = await _shoppingListService.GetByIdAsync(id, cancellationToken);
            return Ok(list);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Cria uma nova lista de compras.
    /// </summary>
    /// <param name="request">Os dados para criação da lista de compras.</param>
    /// <returns>Retorna a lista de compras criada.</returns>
    // POST api/shoppinglists
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateShoppingListRequest request, CancellationToken cancellationToken)
    {
        var created = await _shoppingListService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    /// <param name="request">Os dados para atualização da lista de compras.</param>
    /// <returns>Retorna a lista de compras atualizada.</returns>
    // PUT api/shoppinglists/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateShoppingListRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _shoppingListService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Deleta ou restaura logicamente uma lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    // DELETE api/shoppinglists/toggledeleted/{id}
    [HttpDelete("toggledeleted/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleDeleted(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.ToggleDeletedAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Deleta uma lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    // DELETE api/shoppinglists/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Adiciona um item a uma lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    /// <param name="request">Os dados para adição do item à lista de compras.</param>
    /// <returns>Retorna o item adicionado à lista de compras.</returns>
    // POST api/shoppinglists/{id}/items
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(ShoppingListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] CreateShoppingListItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _shoppingListService.AddItemAsync(id, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Atualiza item da lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    /// <param name="itemId">O ID do item da lista de compras.</param>
    /// <param name="request">Os dados para atualização do item da lista de compras.</param>
    /// <returns>Retorna o item da lista de compras atualizado.</returns>
    // PUT api/shoppinglists/{id}/items/{itemId}
    [HttpPut("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(typeof(ShoppingListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid id, Guid itemId, [FromBody] UpdateShoppingListItemRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _shoppingListService.UpdateItemAsync(id, itemId, request, cancellationToken);
            return Ok(item);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }

    /// <summary>
    /// Deleta um item da lista de compras existente.
    /// </summary>
    /// <param name="id">O ID da lista de compras.</param>
    /// <param name="itemId">O ID do item da lista de compras.</param>
    // DELETE api/shoppinglists/{id}/items/{itemId}
    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid id, Guid itemId, CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.DeleteItemAsync(id, itemId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}