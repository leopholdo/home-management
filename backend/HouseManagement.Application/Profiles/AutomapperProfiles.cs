using AutoMapper;
using HouseManagement.Application.DTOs.ShoppingList;
using HouseManagement.Domain.Entities;

namespace HouseManagement.Application.Profiles;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<ShoppingList, ShoppingListDto>();
        CreateMap<ShoppingListItem, ShoppingListItemDto>();
    }
}