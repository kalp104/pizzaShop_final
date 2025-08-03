using System;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionMenu))]
public class MenuController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    private readonly IRoleService _roleService;
    private readonly IMenuService _menuService;

    public MenuController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService,
        IRoleService roleService,
        IMenuService menuService
    )
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
        _roleService = roleService;
        _menuService = menuService;
    }
    public async Task FetchData()
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.role = role;
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
    }

    public async Task<IActionResult> Index(
        int? categoryId = null,
        string? searchTerm = null,
        int? modifierGroupId = null,
        string? searchModifier = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(
            categoryId,
            searchTerm,
            pageNumber,
            pageSize
        );
        MenuWithItemsViewModel menu2 = await _menuService.GetModifiers(
            modifierGroupId,
            searchTerm,
            pageNumber,
            pageSize
        );
        ViewBag.SelectedCategoryId = categoryId;
        MenuWithItemsViewModel result = new MenuWithItemsViewModel
        {
            Categories = menu.Categories,
            Items = menu.Items,
            CurrentPage = menu.CurrentPage,
            PageSize = menu.PageSize,
            TotalItems = menu.TotalItems,
            modifiergroups = menu2.modifiergroups,
            Modifiers = menu2.Modifiers,
            CurrentPage1 = menu2.CurrentPage1,
            PageSize1 = menu2.PageSize1,
            TotalItems1 = menu2.TotalItems1,
        };
        ViewBag.SelectedModifierId = modifierGroupId;

        return View(result);
    }
   
    #region CATEGORY
    // CATEGORY
    public async Task<IActionResult> FilterItems(
        int? categoryId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(
            categoryId,
            searchTerm,
            pageNumber,
            pageSize
        );
        return PartialView("_ItemsPartial", menu);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        List<Category>? category =  await _menuService.GetAllCategories();
        var cat = category.Select(c => new
        {
            categoryid = c.Categoryid,
            categoryname = c.Categoryname,
            categorydescription = c.Categorydescription
        }).ToList();    
        return Json(new { categories = cat });
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(MenuWithItemsViewModel model)
    {
        try{
            await FetchData();
            bool res = await _menuService.AddCategoryService(model);
            if (res)
            {
                return Json(new
                {
                    success = true,
                    message = "Category added successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Category already exists"
                });
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(MenuWithItemsViewModel model)
    {
        try{
            await FetchData();
            bool res = await _menuService.EditCategoryService(model);
            
            if(res)
            {
                return Json(new {
                    success = true,
                    message = "Category Edited successfully"
                });
            }  
            else
            {
                return Json(new {
                    success = false,
                    message = "Category name already exists!"
                }); 
            } 
                 
        }
        catch( Exception e)
        {
             System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
        
        
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCategory(MenuWithItemsViewModel model)
    {
        try{
            await FetchData();
            bool res = await _menuService.DeleteCategoryService(model);
            
            if(res)
            {
                return Json(new {
                    success = true,
                    message = "Category deleted successfully"
                });
            }
                
            else 
            {
                return Json(new {
                    success = false,
                    message = "Error in deleting Category!"
                }); 
            }
                
        }catch( Exception e)
        {
            System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
    }
    
    #endregion
    #region  ITEM
    // ITEM
    [HttpGet]
    public async Task AddItem()
    {
        await FetchData();
        List<Modifiergroup>? res = await _menuService.GetAllModifierGroup();
        MenuWithItemsViewModel result = new MenuWithItemsViewModel();
        result.modifiergroups = res;
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<IActionResult> AddItem(
        MenuWithItemsViewModel viewModel
    )
    {
        if (viewModel.item == null)
        {
            MenuWithItemsViewModel menu2 = await _menuService.GetAllCategory(0, "", 1, 5);
            menu2.item = viewModel.item;
            await FetchData();
            ModelState.AddModelError("", "Item details are required.");
            return View("Index", menu2);
        }

        if (viewModel.item != null)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var modifierGroups = JsonSerializer.Deserialize<
                    Dictionary<string, ModifierGroupDataHelperViewModel>
                >(viewModel.selectedModifierGroups, options);

                await FetchData();
                bool  res = await _menuService.AddItemAsync(
                    viewModel,
                    viewModel.item?.UploadFiles,
                    ViewBag.Userid,
                    modifierGroups
                );
                if(res)
                {
                    return Json(new{
                        success = true,
                        message = "Item added successfully!"
                    });
                }else{
                    return Json(new{
                        success = false,
                        message = "item name is already exist!"
                    });
                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return Json(new{
                    success = false,
                    message = ex.Message
               });
                
            }
        }

        MenuWithItemsViewModel menu = await _menuService.GetAllCategory(0, "", 1, 5);
        menu.item = viewModel.item;
        return RedirectToAction("Index", menu);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(int userid, int itemid)
    {
        try{
            bool res = await _menuService.DeleteItemService(userid, itemid);
            if(res)
                {
                    return Json(new{
                            success = true,
                            message = "Item Deleted successfully!"
                        });

                }else{
                    return Json(new{
                            success = false,
                            message = "Error in Item Delete!"
                        });
                }
        }
        catch( Exception e)
        {
            System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultipleItems(string selectedItemIds)
    {
        if (string.IsNullOrEmpty(selectedItemIds))
        {
            return Json(new { success = false, message = "No items selected for deletion." });
        }

        try
        {
            // Parse the comma-separated string into an array of integers
            List<int> itemsIds = selectedItemIds.Split(',').Select(id => int.Parse(id)).ToList();

            if (!itemsIds.Any())
            {
                return Json(new { success = false, message = "Invalid item IDs provided." });
            }

            await FetchData();
            int userId = ViewBag.Userid; // Ensure this is set correctly (e.g., from session or claims)
            bool res = await _menuService.DeleteMultipleItemsAsync(itemsIds, userId);
            if(res)
            {
                return Json(new{
                        success = true,
                        message = "selected Item Deleted successfully!"
                    });

            }else{
                return Json(new{
                        success = false,
                        message = "Error in selected Item Delete!"
                    });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Failed to delete items: {ex.Message}" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> IsAvailableUpdate(int itemId, bool available)
    {
        try
        {
            await FetchData();
            int userId = ViewBag.Userid; // Consider dependency injection instead
            Item item = await _menuService.IsAvailableUpdateAsync(itemId, available, userId) ?? new Item();
            
            if (item.Isavailable == available) // Removed null check
            {
                return Json(new { success = true, data = "Update completed successfully" });
            }

            return Json(new { success = false, data = "Update failed to apply" });
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Error in IsAvailableUpdate: " + e.Message);
            return Json(new { success = false, data = "An error occurred: " + e.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditItemPartial(int id)
    {
        try
        {
            var item = await _menuService.GetItemById(id);
            if (item == null)
            {
                return NotFound("Item not found.");
            }

            MenuWithItemsViewModel viewModel = new MenuWithItemsViewModel
            {
                item = new ItemsViewModel
                {
                    Itemid = item.Itemid,
                    Categoryid = item.Categoryid,
                    Itemname = item.Itemname,
                    Itemtype = item.Itemtype,
                    Rate = item.Rate,
                    Quantity = item.Quantity,
                    Units = item.Units,
                    Isavailable = (bool)item.Isavailable,
                    Defaulttax = (bool)item.DefaultTax,
                    Taxpercentage = item.Taxpercentage,
                    Shortcode = item.Shortcode,
                    Description = item.Description,
                    ImageUrl = item.Imageid ?? "",
                },
                Categories = await _menuService.GetAllCategories(),
                modifiergroups = await _menuService.GetAllModifierGroup(),
            };

            await FetchData();
            return PartialView("_EditItemPartial", viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in EditItemPartial: " + ex.Message);
            return Json(
                new { success = false, message = $"Error loading edit form: {ex.Message}" }
            );
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<IActionResult> EditItem(MenuWithItemsViewModel viewModel)
    {

        if (viewModel.item == null)
        {
            return Json(new { success = false, message = "Item details are required." });
        }

        try
        {
            await FetchData();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            Dictionary<string, ModifierGroupDataHelperViewModel>? modifierGroups =
                string.IsNullOrEmpty(viewModel.selectedModifierGroups)
                    ? new Dictionary<string, ModifierGroupDataHelperViewModel>()
                    : JsonSerializer.Deserialize<
                        Dictionary<string, ModifierGroupDataHelperViewModel>
                    >(viewModel.selectedModifierGroups, options);

            bool res = await _menuService.UpdateItemAsync(
                viewModel,
                viewModel.item.UploadFiles,
                viewModel.Userid,
                modifierGroups
            );

            if (res)
            {
                return Json(new { success = true, message = "Selected items edited successfully!" });
            }
            else
            {
                TempData["ErrorMessage"] = "Item name already exists";
                return Json(new { success = false, message = "Item name already exists" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in EditItem: " + ex.Message);
            return Json(new { success = false, message = "Error updating item: " + ex.Message });
        }
    }

    #endregion
    #region MODIFIER GROUP
    // MODIFIER GROUP
    
    [HttpGet]
    public async Task<IActionResult> GetModifierGroupsAside()
    {
        List<Modifiergroup>? modifierGroup = await _menuService.GetAllModifierGroup();
        var modGroup = modifierGroup.Select(m => new
        {
            modifiergroupid = m.Modifiergroupid,
            modifiergroupname = m.Modifiergroupname,
            modifiergroupdescription = m.Modifiergroupdescription
        }).ToList();
        return Json(new { modifierGroups = modGroup });
    }

    public async Task<IActionResult> FilterModifiers(
        int? modifierGroupId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetModifiers(
            modifierGroupId,
            searchTerm,
            pageNumber,
            pageSize
        );
        return PartialView("_ModifiersPartial", menu);
    }
    
    public async Task<IActionResult> FilterModifiersAtAddCategory(
        int? modifierGroupId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetModifiers(
            modifierGroupId,
            searchTerm,
            pageNumber,
            pageSize
        );
        return PartialView("_ModifiersAtAddModifierGroupPartial", menu);
    }
    
    public async Task<IActionResult> FilterModifiersAtEditModifierGroup(
        int? modifierGroupId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        MenuWithItemsViewModel menu = await _menuService.GetModifiers(
            modifierGroupId,
            searchTerm,
            pageNumber,
            pageSize
        );
        return PartialView("_ModifiersAtEditModifierGroup", menu);
    }

    [HttpGet]
    public async Task<IActionResult> GetModifiersByGroup(int modifierGroupId)
    {
        try
        {
            List<ModifierDtoViewModel>? modifiers =
                await _menuService.GetModifiersByModifierGroupId(modifierGroupId);
            return Json(modifiers);
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("error in fatching modifiers is add item : " + ex.Message);
            return StatusCode(500, new { error = "Error fetching modifiers" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddModifierGroup(MenuWithItemsViewModel model)
    {
        
        try{
            List<int> modifierIds = string.IsNullOrEmpty(model.selectedIds)
                ? new List<int>()
                : model.selectedIds.Split(',').Select(int.Parse).ToList();

            model.SelectedModifierIds = modifierIds; 
            await FetchData();
            bool res = await _menuService.AddModifierGroupService(model);
        
            if (res)
            {
                return Json(new
                {
                    success = true,
                    message = "ModifierGroup added successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "ModifierGroup name already exists!"
                });
            }
        }catch( Exception e)
        {
             System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModifierGroup(MenuWithItemsViewModel model)
    {
        try{
            await FetchData();
            bool res = await _menuService.DeleteModifierGroupService(model);
            TempData["ModifierGroupAdd"] = "";
                if (res)
                {
                    return Json(new
                    {
                        success = true,
                        message = "ModifierGroup Deleted successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error in deleting ModifierGroup!"
                    });
                }
        }catch( Exception e)
        {
            System.Console.WriteLine(e.Message);
            return Json(new
                {
                    success = false,
                    message = e.Message
                });
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddModifierGroupDetails(string selectedIds)
    {
        try
        {
            List<int>? modifierIds = selectedIds.Split(',').Select(id => int.Parse(id)).ToList();
            List<Modifier>? modifiers = await _menuService.GetModifiersListAsync(modifierIds); // Fetch modifiers from service

            return Json(new { success = true, modifiers = modifiers });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error adding modifiers: " + ex.Message });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetModifierGroup(int id)
    {
        MenuWithItemsViewModel? modifierGroup = await _menuService.GetModifierGroupById(id);
        if (modifierGroup == null)
            return NotFound();

        return Json(
            new
            {
                modifiergroupid = modifierGroup.Modifiergroupid,
                modifiergroupname = modifierGroup.Modifiergroupname,
                modifiergroupdescription = modifierGroup.Modifiergroupdescription,
                selectedModifiers = modifierGroup.selectedModifiersViewModels?.Select(m => new
                {
                    modifierId = m.ModifierId,
                    modifierName = m.ModifierName,
                }),
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditModifierGroup(MenuWithItemsViewModel model)
    {
        try
        {
            List<int> modifierIds = string.IsNullOrEmpty(model.selectedIds1)
                ? new List<int>()
                : model.selectedIds1.Split(',').Select(int.Parse).ToList();

            model.SelectedModifierIds = modifierIds;
            bool res = await _menuService.UpdateModifierGroupService(model);

            if (res)
            {
                return Json(new
                {
                    success = true,
                    message = "Modifier Group updated successfully"
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Modifier Group name already exists"
                });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }


    #endregion
    #region MODIFIERS
    // MODIFIERS
    
    [HttpPost]
    public async Task<IActionResult> AddModifier(MenuWithItemsViewModel viewModel)
    {
        if (viewModel.modifiersViewModel == null)
        {
            MenuWithItemsViewModel menu2 = await _menuService.GetModifiers(0, "", 1, 5);
            menu2.modifiersViewModel = viewModel.modifiersViewModel;
            ModelState.AddModelError(
                "",
                "Modifier details and at least one modifier group are required."
            );
            return View("Index", menu2);
        }

        try
        {
            await FetchData(); // Assuming this sets ViewBag.Userid
            viewModel.Userid = ViewBag.Userid;
            bool res = await _menuService.AddModifierAsync(viewModel);

            if(res)
                {
                    return Json(new{
                        success = true,
                        message = "Modifier added successfully!"
                    });
                }else{
                    return Json(new{
                        success = false,
                        message = "Modifier name already exists!"
                    });
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding modifier: {ex.Message}");
            return View("Index", viewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteModifier(int modifierid)
    {
        try
        {
            await FetchData();
            bool res = await _menuService.DeleteModifierService(modifierid, ViewBag.Userid);
            
            if(res)
            {
                return Json(new{
                        success = true,
                        message = "Modifier deleted successfully"
                    });

            }else{
                return Json(new{
                        success = false,
                        message = "Error while deleting Modifier"
                    });
            }

            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting modifier: {ex.Message}");
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMultipleModifiers(string selectedModifierIds)
    {
        if (string.IsNullOrEmpty(selectedModifierIds))
        {
            return Json(new { success = false, message = "No modifiers selected for deletion." });
        }

        try
        {
            List<int> modifierIds = selectedModifierIds
                .Split(',')
                .Select(id => int.Parse(id))
                .ToList();

            if (!modifierIds.Any())
            {
                return Json(new { success = false, message = "Invalid modifier IDs provided." });
            }

            await FetchData();
            int userId = ViewBag.Userid;
            bool res = await _menuService.DeleteMultipleModifiersAsync(modifierIds, userId);
             if(res)
            {
                return Json(new{
                        success = true,
                        message = "Modifier deleted successfully"
                    });

            }else{
                return Json(new{
                        success = false,
                        message = "Error while deleting Modifier"
                    });
            }
        }
        catch (Exception ex)
        {
            return Json(
                new { success = false, message = $"Failed to delete modifiers: {ex.Message}" }
            );
        }
    } 
    
    [HttpGet]
    public async Task<IActionResult> GetModifiersByItemId(int itemId)
    {
        try
        {
            List<ModifierGroupDataHelperViewModel>? modifierGroups =
                await _menuService.GetModifierGroupsByItemId(itemId);
            var result = modifierGroups
                .Select(group => new
                {
                    modifierGroupId = group.ModifierGroupid,
                    modifierGroupName = group.Modifiergroupname,
                    minValue = group.MinValue ?? 0,
                    maxValue = group.MaxValue ?? 0,
                    modifiers = group
                        .Modifiers?.Select(m => new
                        {
                            modifierId = m.ModifierId,
                            modifierName = m.Modifiername,
                            modifierRate = m.Modifierrate,
                        })
                        .ToList(),
                })
                .ToList();
            return Json(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching modifiers for item {itemId}: {ex.Message}");
            return Json(
                new { success = false, message = $"Failed to fetch modifiers: {ex.Message}" }
            );
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetModifierData(int modifierId)
    {
        try
        {
            var modifier = await _menuService.GetModifierByIdAsync(modifierId);
            if (modifier == null)
            {
                return NotFound();
            }

            var currentGroupIds = await _menuService.GetModifierGroupIdsAsync(modifierId);
            var data = new
            {
                modifierid = modifier.Modifierid,
                modifiername = modifier.Modifiername,
                modifierrate = modifier.Modifierrate,
                modifierquantity = modifier.Modifierquantity,
                modifierunit = modifier.Modifierunit,
                modifierdescription = modifier.Modifierdescription,
                modifiergroupid = currentGroupIds,
            };

            return Json(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching modifier data: {ex.Message}");
            return Json(
                new { success = false, message = $"Failed to fetch modifiers: {ex.Message}" }
            );
        }
    }
    
    // POST: Edit Modifier
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditModifier(MenuWithItemsViewModel viewModel)
    {
        try
        {
            if (
                viewModel.modifiersViewModel == null
                || string.IsNullOrEmpty(viewModel.modifiersViewModel.Modifiername)
                || viewModel.modifiersViewModel?.Modifiergroupid?.Count == 0
            )
            {
                return Json(
                    new
                    {
                        success = false,
                        message = "Name and at least one modifier group are required.",
                    }
                );
            }

            await FetchData(); // Assuming this sets ViewBag.Userid
            viewModel.Userid = ViewBag.Userid;
            bool res = await _menuService.EditModifierAsync(viewModel);
                if(res)
                {
                    return Json(new { success = true,message = "Modifier updated successfully!" });
                }else{
                    return Json(new { success = false,message="Modifier name already exists" });
                }
           
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating modifier: {ex.Message}");
            return Json(new { success = false, message = ex.Message });
        }
    }

    #endregion
}
