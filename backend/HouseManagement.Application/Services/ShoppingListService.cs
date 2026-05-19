using AutoMapper;
using HouseManagement.Application.DTOs.ShoppingList;
using HouseManagement.Application.Interfaces.Repositories;
using HouseManagement.Application.Interfaces.Services;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Application.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IShoppingListItemRepository _shoppingListItemRepository;
    private readonly IMapper _mapper;

    public ShoppingListService(
        IShoppingListRepository shoppingListRepository,
        IShoppingListItemRepository shoppingListItemRepository,
        IMapper mapper)
    {
        _shoppingListRepository = shoppingListRepository;
        _shoppingListItemRepository = shoppingListItemRepository;
        _mapper = mapper;
    }


    public async Task<IEnumerable<ShoppingListSummaryDto>> GetAllAsync(bool isDeleted, CancellationToken cancellationToken = default)
    {
        var lists = await _shoppingListRepository.GetAllAsync(isDeleted, cancellationToken);
        return _mapper.Map<IEnumerable<ShoppingListSummaryDto>>(lists);
    }

    public async Task<ShoppingListDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{id}' was not found.");

        return _mapper.Map<ShoppingListDto>(shoppingList);
    }

    public async Task<ShoppingListDto> CreateAsync(CreateShoppingListRequest request, CancellationToken cancellationToken = default)
    {
        var shoppingList = new ShoppingList
        {
            Name = request.Name,
            Notes = request.Notes,
            IsCompleted = false
        };

        var created = await _shoppingListRepository.AddAsync(shoppingList, cancellationToken);
        return _mapper.Map<ShoppingListDto>(created);
    }

    public async Task<ShoppingListDto> UpdateAsync(Guid id, UpdateShoppingListRequest request, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{id}' was not found.");

        shoppingList.Name = request.Name;
        shoppingList.Notes = request.Notes;
        shoppingList.IsCompleted = request.IsCompleted;

        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
        return _mapper.Map<ShoppingListDto>(shoppingList);
    }

    public async Task ToggleDeletedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{id}' was not found.");

        await _shoppingListRepository.ToggleDeletedAsync(shoppingList, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{id}' was not found.");

        await _shoppingListRepository.DeleteAsync(shoppingList, cancellationToken);
    }

    public async Task<ShoppingListItemDto> AddItemAsync(Guid shoppingListId, CreateShoppingListItemRequest request, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(shoppingListId, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{shoppingListId}' was not found.");

        var item = new ShoppingListItem
        {
            Name = request.Name,
            Quantity = request.Quantity,
            Notes = request.Notes,
            IsCompleted = false
        };

        shoppingList.Items.Add(item);
        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);

        return _mapper.Map<ShoppingListItemDto>(item);
    }

    public async Task<ShoppingListItemDto> UpdateItemAsync(Guid shoppingListId, Guid itemId, UpdateShoppingListItemRequest request, CancellationToken cancellationToken = default)
    {
        _ = await _shoppingListRepository.GetByIdAsync(shoppingListId, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{shoppingListId}' was not found.");

        var item = await _shoppingListItemRepository.GetByIdAsync(itemId, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingListItem with id '{itemId}' was not found.");

        if (item.ShoppingListId != shoppingListId)
            throw new InvalidOperationException($"Item '{itemId}' does not belong to ShoppingList '{shoppingListId}'.");

        item.Name = request.Name;
        item.Quantity = request.Quantity;
        item.IsCompleted = request.IsCompleted;
        item.Notes = request.Notes;

        await _shoppingListItemRepository.UpdateAsync(item, cancellationToken);
        return _mapper.Map<ShoppingListItemDto>(item);
    }

    public async Task DeleteItemAsync(Guid shoppingListId, Guid itemId, CancellationToken cancellationToken = default)
    {
        _ = await _shoppingListRepository.GetByIdAsync(shoppingListId, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingList with id '{shoppingListId}' was not found.");

        var item = await _shoppingListItemRepository.GetByIdAsync(itemId, cancellationToken)
            ?? throw new KeyNotFoundException($"ShoppingListItem with id '{itemId}' was not found.");

        if (item.ShoppingListId != shoppingListId)
            throw new InvalidOperationException($"Item '{itemId}' does not belong to ShoppingList '{shoppingListId}'.");

        await _shoppingListItemRepository.DeleteAsync(item, cancellationToken);
    }
}