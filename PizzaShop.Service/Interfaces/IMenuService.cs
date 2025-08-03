using System;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<MenuWithItemsViewModel> GetAllCategory(
        int? categoryId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    );
    Task<List<Category>> GetAllCategories();
    Task<List<Modifiergroup>> GetAllModifierGroup();
    Task<MenuWithItemsViewModel> GetModifiers(
        int? modifierGroupId = null,
        string? searchModifier = null,
        int pageNumber = 1,
        int pageSize = 5
    );
    Task<bool> AddCategoryService(MenuWithItemsViewModel model);
    Task<bool> AddModifierGroupService(MenuWithItemsViewModel model);
    Task<bool> EditCategoryService(MenuWithItemsViewModel model);
    Task<bool> DeleteCategoryService(MenuWithItemsViewModel model);
    Task<bool> DeleteModifierGroupService(MenuWithItemsViewModel model);
    Task<bool> DeleteItemService(int userid, int itemid);
    Task<bool> AddItemAsync(
        MenuWithItemsViewModel viewModel,
        IFormFile? uploadFile,
        int userId,
        Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups
    );
    Task<bool> AddModifierAsync(MenuWithItemsViewModel viewModel);
    Task<bool> UpdateItemAsync(
        MenuWithItemsViewModel viewModel,
        IFormFile? uploadFile,
        int userId,
        Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups
    );
    Task<Item?> GetItemById(int id);
    Task<Modifier?> GetModifierById(int id);
    Task<List<Modifier>> GetModifiersListAsync(List<int> id);
    Task<Item> IsAvailableUpdateAsync(int id, bool available, int userid);
    Task<bool> DeleteMultipleItemsAsync(List<int>? itemIds, int userId);
    Task<bool> DeleteMultipleModifiersAsync(List<int>? modifierIds, int userId);
    Task<bool> DeleteModifierService(int modifierid, int Userid);

    Task<Modifier?> GetModifierByIdAsync(int modifierId);
    Task<List<Modifiergroup>> GetAllModifierGroupsAsync();
    Task<List<int>> GetModifierGroupIdsAsync(int modifierId);
    Task<bool> EditModifierAsync(MenuWithItemsViewModel viewModel);
    Task<List<ModifierDtoViewModel>> GetModifiersByModifierGroupId(int modifierGroupId);
    Task<List<ModifierGroupDataHelperViewModel>?> GetModifierGroupsByItemId(int itemId);

    Task<MenuWithItemsViewModel?> GetModifierGroupById(int modifierGroupId);
    Task<bool> UpdateModifierGroupService(MenuWithItemsViewModel model);
    Task<List<ModifierViewModel>> GetModifiersByIds(List<int> modifierIds);

    Task<List<Item>?> GetItemsByCategoryId(int? category = null, string? searchTerm = null);
}
