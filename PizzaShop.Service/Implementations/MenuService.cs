using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class MenuService : IMenuService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly ICategoryRepository _catrgoryRepository;
    private readonly IModifierGroupRepository _modifierGroupRepository;

    public MenuService(
        IWebHostEnvironment webHostEnvironment,
        IItemRepository itemRepository,
        IModifierRepository modifierRepository,
        ICategoryRepository catrgoryRepository,
        IModifierGroupRepository modifierGroupRepository
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
        _catrgoryRepository = catrgoryRepository;
        _modifierGroupRepository = modifierGroupRepository;
    }

    public async Task<MenuWithItemsViewModel> GetAllCategory(
        int? categoryId = null,
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        List<Category>? categories = await _catrgoryRepository.GetAllCategories();
        List<Item>? items;

        // Filter by category first
        if (categoryId.HasValue)
        {
            items = await _itemRepository.GetItemsByCategoryId(categoryId.Value);
        }
        else
        {
            items = await _itemRepository.GetAllItems();
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            items = items.Where(i => i.Itemname.ToLower().Contains(searchTerm)).ToList();
        }

        // Get total count before pagination
        int totalItems = items.Count;

        // Apply pagination
        items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new MenuWithItemsViewModel
        {
            Categories = categories,
            Items = items,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems, 
        };
    }

    public async Task<List<Category>> GetAllCategories()
    {
        List<Category>? categories = await _catrgoryRepository.GetAllCategories();
        return categories;
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroup()
    {
        List<Modifiergroup>? mg = await _modifierGroupRepository.GetAllModifierGroups();
        return mg;
    }
    

    public async Task<MenuWithItemsViewModel> GetModifiers(
        int? modifierGroupId = null,
        string? searchModifier = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        List<Modifiergroup>? mg = await _modifierGroupRepository.GetAllModifierGroups();
        List<Modifier>? modifiers;

        // Filter items by category if specified
        if (modifierGroupId.HasValue)
        {
            modifiers = await _modifierRepository.GetModifiersByMG(modifierGroupId.Value);
        }
        else
        {
            modifiers = await _modifierRepository.GetAllModifiers();
        }
        // Apply search filter if a search term is provided
        if (!string.IsNullOrEmpty(searchModifier))
        {
            searchModifier = searchModifier.ToLower();
            modifiers = modifiers
                .Where(i => i.Modifiername.ToLower().Contains(searchModifier))
                .ToList();
        }
        int totalItems = modifiers.Count;
        modifiers = modifiers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new MenuWithItemsViewModel
        {
            modifiergroups = mg,
            Modifiers = modifiers,
            CurrentPage1 = pageNumber,
            PageSize1 = pageSize,
            TotalItems1 = totalItems,
        };
    }

    public async Task<bool> AddCategoryService(MenuWithItemsViewModel model)
    {
        try{
            if(model.CategoryName!=null)
            {
                List<Category>? checkCategory = await _catrgoryRepository.CheckCategoryByName(model.CategoryName.Trim().ToLower());
                if(checkCategory != null && checkCategory.Count > 0 )
                {
                    return false;
                }

            }
            Category category = new Category
            {
                Categorydescription = model.CategoryDescription,
                Categoryname = model.CategoryName,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
                Createdbyid = model.Userid,
                Editedbyid = model.Userid,
                Isdeleted = false,
            };
            await _catrgoryRepository.AddCategory(category);

            return true;
        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Errorcode 500: internal server error");
        }
        
    }

    public async Task<bool> AddModifierGroupService(MenuWithItemsViewModel model)
    {
        try{
            if(model.Modifiergroupname!=null)
            {
                List<Modifiergroup>? checkModifierGroup = await _modifierGroupRepository.CheckModifierGroupByName(model.Modifiergroupname.Trim().ToLower());
                if(checkModifierGroup != null && checkModifierGroup.Count > 0 )
                {
                    return false;
                }

            }

            Modifiergroup modifiergroup = new Modifiergroup
            {
                Modifiergroupdescription = model.Modifiergroupdescription,
                Modifiergroupname = model.Modifiergroupname,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
                Createdbyid = model.Userid,
                Editedbyid = model.Userid,
            };
            await _modifierGroupRepository.AddModifierGroup(modifiergroup);

            List<int>? list = model.SelectedModifierIds;
            if (list != null)
            {
                foreach (int l in list)
                {
                    Modifier? m = await _modifierRepository.GetModifierById(l);
                    if (m != null)
                    {
                        Modfierandgroupsmapping? finding =
                            await _modifierGroupRepository.GetMappingsById(
                                modifiergroup.Modifiergroupid,
                                l
                            );
                        if (finding != null)
                        {
                            continue;
                        }
                        else
                        {
                            Modfierandgroupsmapping mapping = new Modfierandgroupsmapping
                            {
                                Modifiergroupid = modifiergroup.Modifiergroupid,
                                Modifierid = l,
                                Createdat = DateTime.Now,
                                Createdbyid = model.Userid,
                            };
                            await _modifierGroupRepository.AddModfierandGroupsMapping(mapping);
                        }
                    }
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> EditCategoryService(MenuWithItemsViewModel model)
    {
        try
        {
            if(model.CategoryName!=null)
            {
                List<Category>? checkCategory = await _catrgoryRepository.CheckCategoryByName(model.CategoryName.Trim().ToLower());
                if(checkCategory != null && checkCategory.Count > 0 )
                {
                    foreach(Category c in checkCategory)
                    {
                        if(c.Categoryid!=model.Categoryid && model.CategoryName.Trim().ToLower().Equals(c.Categoryname.Trim().ToLower()))
                        {
                            return false;
                        }
                    }
                }

            }
            Category? category = await _catrgoryRepository.GetCategoryById(model.Categoryid);
            if (category != null)
            {
                category.Categorydescription = model.CategoryDescription;
                category.Categoryname = model.CategoryName;
                category.Editedat = DateTime.Now;
                category.Editedbyid = model.Userid;
                await _catrgoryRepository.UpdateCategory(category);
            }
            return true;

        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error code 500: internal server error");
        }
    }

    public async Task<bool> DeleteCategoryService(MenuWithItemsViewModel model)
    {
        try{
            Category? category = await _catrgoryRepository.GetCategoryById(model.Categoryid);
            List<Item> items = await _itemRepository.GetAllItems();
            if (category != null)
            {
                category.Isdeleted = true;
                category.Deletedat = DateTime.Now;
                category.Deletedbyid = model.Userid;
                await _catrgoryRepository.UpdateCategory(category);
            }
            foreach (Item i in items)
            {
                if (i.Categoryid == category?.Categoryid)
                {
                    i.Isdeleted = true;
                    i.Deletedat = DateTime.Now;
                    i.Deletedbyid = model.Userid;
                    await _itemRepository.UpdateItem(i);
                }
            }
            return true;
        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> DeleteModifierGroupService(MenuWithItemsViewModel model)
    {
        try{
            Modifiergroup? mg = await _modifierGroupRepository.GetModifierGroupById(model.Modifiergroupid);
            List<Modfierandgroupsmapping> modifiers = await _modifierGroupRepository.GetAllModfierandgroupsmapping();
            if (mg != null)
            {
                mg.Isdeleted = true;
                mg.Deletedat = DateTime.Now;
                mg.Deletedbyid = model.Userid;
                await _modifierGroupRepository.UpdateModifierGroup(mg);
            }
            foreach (Modfierandgroupsmapping i in modifiers)
            {
                if (i.Modifiergroupid == mg.Modifiergroupid)
                {
                    i.Isdelete = true;
                    i.Deletedat = DateTime.Now;
                    i.Deletedbyid = model.Userid;
                    await _modifierGroupRepository.UpdateModfierandGroupsMapping(i);
                }
            }
            List<ItemModifiergroupMapping>? itemModifiergroupMappings =
                await _modifierGroupRepository.GetByModifierGroupIdMappingAsync(model.Modifiergroupid);
            if (itemModifiergroupMappings != null)
            {
                foreach (var i in itemModifiergroupMappings)
                {
                    await _modifierGroupRepository.DeleteItemModifierGroupMapping(i);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> DeleteItemService(int userid, int itemid)
    {
        try{
            Item? item = await _itemRepository.GetItemById(itemid);
            if (item != null)
            {
                item.Isdeleted = true;
                item.Deletedat = DateTime.Now;
                item.Deletedbyid = userid;
                await _itemRepository.UpdateItem(item);
            }
            return true;
        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> DeleteModifierService(int modifierid, int userid)
    {
        try{
            List<Modfierandgroupsmapping>? mappings = await _modifierGroupRepository.GetByModifierIdAsync(modifierid); 
            if(mappings != null)
            {
                foreach (Modfierandgroupsmapping m in mappings)
                {
                    await _modifierGroupRepository.DeleteModfierandGroupsMapping(m);   
                }
            }
            

            Modifier? item = await _modifierRepository.GetModifierById(modifierid);
            if (item != null)
            {
                item.Isdeleted = true;
                item.Deletedat = DateTime.Now;
                item.Deletedbyid = userid;
                await _modifierRepository.UpdateModifier(item);
            }
        return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
    }

    public async Task<bool> DeleteMultipleItemsAsync(List<int>? itemIds, int userId)
    {
        try{
            if (itemIds == null || !itemIds.Any())
            {
                throw new ArgumentException("No item IDs provided for deletion.");
            }

            foreach (var itemId in itemIds)
            {
                Item? item = await _itemRepository.GetItemById(itemId); // Assume an IItemsRepository
                if (item != null)
                {
                    item.Isdeleted = true; // Soft delete
                    item.Deletedbyid = userId;
                    item.Deletedat = DateTime.Now;
                    await _itemRepository.UpdateItem(item);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> DeleteMultipleModifiersAsync(List<int>? modifierIds, int userId)
    {
        try{
            if (modifierIds == null || !modifierIds.Any())
            {
                throw new ArgumentException("No modifier IDs provided for deletion.");
            }

            foreach (int modifierId in modifierIds)
            {
                Modifier? modifier = await _modifierRepository.GetModifierById(modifierId); // Assume an IModifiersRepository
                if (modifier != null)
                {
                    modifier.Isdeleted = true; // Soft delete
                    modifier.Editedbyid = userId;
                    modifier.Editedat = DateTime.Now;
                    await _modifierRepository.UpdateModifier(modifier);
                }
            }
            return true;
        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<bool> AddModifierAsync(MenuWithItemsViewModel viewModel)
    {
            if(viewModel.modifiersViewModel!=null && viewModel.modifiersViewModel.Modifiername!=null)
            {
                List<Modifier>? checkModifier = await _modifierRepository.CheckModifierByName(viewModel.modifiersViewModel.Modifiername.Trim().ToLower());
                if(checkModifier != null && checkModifier.Count > 0 )
                {
                    return false;
                }

            }    


        List<Category> c = await _catrgoryRepository.GetAllCategories();
        viewModel.Categories = c;

        try
        {
            Modifier modifier = new Modifier
            {
                Modifiername = viewModel?.modifiersViewModel?.Modifiername,
                Modifierrate = viewModel?.modifiersViewModel?.Modifierrate,
                Modifierquantity = viewModel?.modifiersViewModel?.Modifierquantity,
                Modifierunit = viewModel?.modifiersViewModel?.Modifierunit,
                Modifierdescription = viewModel?.modifiersViewModel?.Modifierdescription,
                Createdat = DateTime.Now,
                Createdbyid = viewModel?.Userid,
                Editedat = DateTime.Now,
                Editedbyid = viewModel?.Userid,
                Isdeleted = false,
                Deletedat = null,
                Deletedbyid = 0,
                Taxdefault = false,
                Taxpercentage = 0,
            };

            await _modifierRepository.AddModifier(modifier);

            if (modifier.Modifierid == 0)
            {
                throw new Exception("Modifier ID was not generated.");
            }
            if (viewModel?.modifiersViewModel?.Modifiergroupid == null)
                return false;
            // Create mappings for each selected Modifiergroupid
            foreach (int groupId in viewModel.modifiersViewModel.Modifiergroupid)
            {
                Modfierandgroupsmapping modfierandgroupsmapping = new Modfierandgroupsmapping();
                modfierandgroupsmapping.Modifierid = modifier.Modifierid;
                modfierandgroupsmapping.Modifiergroupid = groupId;
                modfierandgroupsmapping.Createdat = DateTime.Now;
                modfierandgroupsmapping.Createdbyid = viewModel.Userid;
                modfierandgroupsmapping.Isdelete = false;

                await _modifierGroupRepository.AddModfierandGroupsMapping(modfierandgroupsmapping);
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            throw new Exception("Error,code 500: internal server error");
        }
    }

    public async Task<Modifier?> GetModifierByIdAsync(int modifierId)
    {
        return await _modifierRepository.GetModifierById(modifierId);
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroupsAsync()
    {
        return await _modifierGroupRepository.GetAllModifierGroups(); 
    }

    public async Task<List<int>> GetModifierGroupIdsAsync(int modifierId)
    {
        List<Modfierandgroupsmapping>? mappings =
            await _modifierGroupRepository.GetByModifierIdAsync(modifierId); 
        return mappings.Select(m => m.Modifiergroupid).ToList();
    }

    public async Task<List<ModifierGroupDataHelperViewModel>?> GetModifierGroupsByItemId(int itemId)
    {
        List<ItemModifiergroupMapping>? imm = await _modifierGroupRepository.GetByItemIdAsync(itemId);

        List<ModifierGroupDataHelperViewModel>? result = new List<ModifierGroupDataHelperViewModel>();
        foreach (var m in imm)
        {
            ModifierGroupDataHelperViewModel r = new ModifierGroupDataHelperViewModel();
            r.MinValue = m.Minvalue;
            r.MaxValue = m.Maxvalue;
            r.ModifierGroupid = m.Modifiergroupid;

            Modifiergroup? mg = await _modifierGroupRepository.GetModifierGroupById(m.Modifiergroupid);
            r.Modifiergroupname = mg.Modifiergroupname;
            List<Modifier>? modifiers = await _modifierGroupRepository.GetModifiersByMGAsync(m.Modifiergroupid);
            r.Modifiers = modifiers
                .Select(m => new ModifierData
                {
                    ModifierId = m.Modifierid,
                    Modifiername = m.Modifiername,
                    Modifierrate = m.Modifierrate,
                })
                .ToList();
            result.Add(r);
        }

        return result;
    }

    public async Task<bool> EditModifierAsync(MenuWithItemsViewModel viewModel)
    {
        try
        {
            if(viewModel.modifiersViewModel!=null && viewModel.modifiersViewModel.Modifiername!=null)
            {
                List<Modifier>? checkModifier = await _modifierRepository.CheckModifierByName(viewModel.modifiersViewModel.Modifiername.Trim().ToLower());
                if(checkModifier != null && checkModifier.Count > 0 )
                {
                    foreach(Modifier s in checkModifier)
                    {
                        if (s.Modifierid != viewModel.modifiersViewModel.Modifierid&& 
                            viewModel.modifiersViewModel.Modifiername != null && 
                            viewModel.modifiersViewModel.Modifiername.Trim().ToLower().Equals(s.Modifiername?.Trim().ToLower()))
                        {
                            return false;
                        }
                    }
                }

            } 


            // Validate input
            if (
                viewModel?.modifiersViewModel == null
                || viewModel.modifiersViewModel.Modifierid == null
            )
            {
                throw new ArgumentException("Invalid modifier data provided.");
            }

            // Fetch the modifier
            Modifier? modifier = await _modifierRepository.GetModifierById(viewModel.modifiersViewModel.Modifierid);
            if (modifier == null)
            {
                throw new Exception("Modifier not found.");
            }

            // Update modifier details
            modifier.Modifiername = viewModel.modifiersViewModel.Modifiername;
            modifier.Modifierrate = viewModel.modifiersViewModel.Modifierrate;
            modifier.Modifierquantity = viewModel.modifiersViewModel.Modifierquantity;
            modifier.Modifierunit = viewModel.modifiersViewModel.Modifierunit;
            modifier.Modifierdescription = viewModel.modifiersViewModel.Modifierdescription;
            modifier.Editedat = DateTime.Now;
            modifier.Editedbyid = viewModel.Userid;

            await _modifierRepository.UpdateModifier(modifier);

            // Fetch existing mappings
            List<Modfierandgroupsmapping>? existingMappings =
                await _modifierGroupRepository.GetByModifierIdAsync(modifier.Modifierid);
            List<int> currentGroupIds =
                existingMappings?.Select(m => m.Modifiergroupid).Distinct().ToList()
                ?? new List<int>();
            List<int> newGroupIds =
                viewModel.modifiersViewModel.Modifiergroupid?.Distinct().ToList()
                ?? new List<int>();

            // Remove mappings that are no longer selected
            if (existingMappings != null)
            {
                foreach (Modfierandgroupsmapping mapping in existingMappings)
                {
                    if (!newGroupIds.Contains(mapping.Modifiergroupid))
                    {
                        await _modifierGroupRepository.DeleteModfierandGroupsMapping(mapping);
                    }
                }
            }

            // Add new mappings (only if they don't already exist)
            foreach (int groupId in newGroupIds)
            {
                if (!currentGroupIds.Contains(groupId))
                {
                    var newMapping = new Modfierandgroupsmapping
                    {
                        Modifierid = modifier.Modifierid,
                        Modifiergroupid = groupId,
                        Createdat = DateTime.Now,
                        Createdbyid = viewModel.Userid,
                        Isdelete = false,
                    };
                    Modfierandgroupsmapping? res = await _modifierGroupRepository.GetMappingsById(
                        newMapping.Modifiergroupid,
                        newMapping.Modifierid
                    );
                    if (res == null)
                    {
                        await _modifierGroupRepository.AddModfierandGroupsMapping(newMapping);
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            throw new Exception("Error,code 500: internal server error");
        }
    }

    public async Task<bool> AddItemAsync(
        MenuWithItemsViewModel viewModel,
        IFormFile? uploadFile,
        int userId,
        Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups
    )
    {
        try{

            if(viewModel != null && viewModel.item != null && viewModel?.item?.Itemname!=null)
            {
                if (!string.IsNullOrEmpty(viewModel?.item?.Itemname))
                {
                    List<Item>? checkItem = await _itemRepository.CheckItemByName(viewModel.item.Itemname.Trim().ToLower());
                
                    if(checkItem != null && checkItem.Count > 0 )
                    {
                        return false;
                    }
                }
            }

            

            List<Category> c = await _catrgoryRepository.GetAllCategories();
            viewModel.Categories = c;
            try
            {
                Item item = new Item
                {
                    Itemname = viewModel.item?.Itemname,
                    Categoryid = viewModel.item.Categoryid,
                    Itemtype = viewModel.item?.Itemtype,
                    Rate = viewModel.item?.Rate,
                    Quantity = viewModel.item?.Quantity,
                    Units = viewModel.item?.Units,
                    Isavailable = viewModel.item?.Isavailable,
                    DefaultTax = viewModel.item.Defaulttax,
                    Taxpercentage = viewModel.item?.Taxpercentage,
                    Shortcode = viewModel.item?.Shortcode,
                    Description = viewModel.item?.Description,
                    Createdat = DateTime.Now,
                    Createdbyid = userId,
                    Status = 1,
                    Isdeleted = false,
                    Editedat = DateTime.Now,
                    Editedbyid = userId,
                    Favourite = false,
                    Deletedbyid = 0,
                };

                if (uploadFile != null && uploadFile.Length > 0)
                {
                    string uniqueFileName =
                        Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "items");
                    Directory.CreateDirectory(uploadFolder);
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await uploadFile.CopyToAsync(fileStream);
                    }

                    item.Imageid = "/items/" + uniqueFileName;
                }
                await _itemRepository.AddItem(item);

                // adding into mapping table of item and modifier groupid :
                foreach (var g in modifierGroups)
                {
                    ItemModifiergroupMapping m = new ItemModifiergroupMapping
                    {
                        Itemid = item.Itemid,
                        Modifiergroupid = int.Parse(g.Key),
                        Minvalue = g.Value.MinValue,
                        Maxvalue = g.Value.MaxValue,
                        Isdeleted = false,
                        Createdat = DateTime.Now,
                        Createdbyid = userId,
                        Editedat = DateTime.Now,
                        Editedbyid = userId,
                    };
                    await _modifierGroupRepository.AddItemModifierGroupMapping(m);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error adding item: " + ex.Message, ex);
                return false;
            }
        }catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
        
    }

    public async Task<List<ModifierDtoViewModel>> GetModifiersByModifierGroupId(int modifierGroupId)
    {
        List<Modfierandgroupsmapping>? mapping =
            await _modifierGroupRepository.GetByModifierGroupIdAsync(modifierGroupId);
        List<int>? modifierIds =
            (mapping != null) ? mapping.Select(u => u.Modifierid).ToList() : new List<int>();
        List<ModifierDtoViewModel> modifiers = new List<ModifierDtoViewModel>();

        if (modifierIds != null)
        {
            foreach (int id in modifierIds)
            {
                Modifier? m = await _modifierRepository.GetModifierById(id);
                if (m != null)
                {
                    modifiers.Add(
                        new ModifierDtoViewModel
                        {
                            Modifierid = m.Modifierid,
                            Modifiername = m.Modifiername,
                            Modifierrate = m.Modifierrate,
                        }
                    );
                }
            }
        }
        return modifiers;
    }

    public async Task<Item?> GetItemById(int id)
    {
        return await _itemRepository.GetItemById(id);
    }

    public async Task<Modifier?> GetModifierById(int id)
    {
        return await _modifierRepository.GetModifierById(id);
    }

    public async Task<List<Modifier>> GetModifiersListAsync(List<int> id)
    {
        List<Modifier>? result = new List<Modifier>();
        foreach (int i in id)
        {
            Modifier? m = await _modifierRepository.GetModifierById(i);
            if (m != null)
                result.Add(m);
        }
        return result;
    }

    public async Task<Item> IsAvailableUpdateAsync(int id, bool available, int userid)
    {
        Item? item = await _itemRepository.GetItemById(id);
        if (item != null)
        {
            item.Isavailable = available;
            item.Editedat = DateTime.Now;
            item.Editedbyid = userid;
            await _itemRepository.UpdateItem(item);
        }
        return item;
    }

    public async Task<bool> UpdateItemAsync(
        MenuWithItemsViewModel viewModel,
        IFormFile? uploadFile,
        int userId,
        Dictionary<string, ModifierGroupDataHelperViewModel> modifierGroups
    )
    {
        try
        {

            if(viewModel != null && viewModel.item != null && viewModel?.item?.Itemname!=null)
            {
                if (!string.IsNullOrEmpty(viewModel?.item?.Itemname))
                {
                    List<Item>? checkItem = await _itemRepository.CheckItemByName(viewModel.item.Itemname.Trim().ToLower());
                
                    if(checkItem != null && checkItem.Count > 0 )
                    {
                        foreach(Item c in checkItem)
                        {
                            if (c.Itemid != viewModel.item.Itemid  && 
                                viewModel.item.Itemname != null && 
                                viewModel.item.Itemname.Trim().ToLower().Equals(c.Itemname?.Trim().ToLower()))
                            {
                                return false;
                            }
                        }
                    }
                }
            }


            if (viewModel?.item == null)
            {
                return false;
            }


            Item? item = await _itemRepository.GetItemById(viewModel.item.Itemid);
            if (item == null)
            {
                return false;
            }

            // Update item properties
            item.Categoryid = viewModel.item.Categoryid;
            item.Itemname = viewModel.item.Itemname;
            item.Itemtype = viewModel.item.Itemtype;
            item.Rate = viewModel.item.Rate;
            item.Quantity = viewModel.item.Quantity;
            item.Units = viewModel.item.Units;
            item.Isavailable = viewModel.item.Isavailable;
            item.DefaultTax = viewModel.item.Defaulttax;
            item.Taxpercentage = viewModel.item.Taxpercentage;
            item.Shortcode = viewModel.item.Shortcode;
            item.Description = viewModel.item.Description;
            item.Editedbyid = userId;
            item.Editedat = DateTime.Now;

            // Handle file upload
            if (uploadFile != null && uploadFile.Length > 0)
            {
                string uniqueFileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "items");
                Directory.CreateDirectory(uploadFolder);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await uploadFile.CopyToAsync(fileStream);
                }

                item.Imageid = "/items/" + uniqueFileName;
            }

            await _itemRepository.UpdateItem(item);

            // Get existing modifier group mappings
            List<ItemModifiergroupMapping>? existingMappings =
                await _modifierGroupRepository.GetByItemIdAsync(item.Itemid);
            List<int> newModifierIds = modifierGroups.Select(u => int.Parse(u.Key)).ToList();

            // Update base item first
            await _itemRepository.UpdateItem(item);

            // Handle modifier group mappings
            if (existingMappings != null && existingMappings.Any())
            {
                // Remove mappings that are no longer needed
                foreach (var existingMapping in existingMappings)
                {
                    if (!newModifierIds.Contains(existingMapping.Modifiergroupid))
                    {
                        await _modifierGroupRepository.DeleteItemModifierGroupMapping(existingMapping);
                    }
                }
            }

            // Add or update modifier group mappings
            foreach (var group in modifierGroups)
            {
                int modifierGroupId = int.Parse(group.Key);
                var existingMapping = existingMappings?.FirstOrDefault(m =>
                    m.Modifiergroupid == modifierGroupId
                );

                if (existingMapping != null)
                {
                    // Update existing mapping
                    existingMapping.Minvalue = group.Value.MinValue;
                    existingMapping.Maxvalue = group.Value.MaxValue;
                    existingMapping.Editedat = DateTime.Now;
                    existingMapping.Editedbyid = userId;
                    existingMapping.Isdeleted = false;
                    await _modifierGroupRepository.UpdateItemModifierGroupMapping(existingMapping);
                }
                else
                {
                    // Add new mapping
                    ItemModifiergroupMapping newMapping = new ItemModifiergroupMapping
                    {
                        Itemid = item.Itemid,
                        Modifiergroupid = modifierGroupId,
                        Minvalue = group.Value.MinValue,
                        Maxvalue = group.Value.MaxValue,
                        Isdeleted = false,
                        Createdat = DateTime.Now,
                        Createdbyid = userId,
                        Editedat = DateTime.Now,
                        Editedbyid = userId,
                    };
                    await _modifierGroupRepository.AddItemModifierGroupMapping(newMapping);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            throw new Exception("Error,code 500: internal server error");
        }
    }

    public async Task<MenuWithItemsViewModel?> GetModifierGroupById(int modifierGroupId)
    {
        Modifiergroup? modifiergroup = await _modifierGroupRepository.GetModifierGroupById(modifierGroupId);
        List<Modfierandgroupsmapping>? modifiersAndGroup =
            await _modifierGroupRepository.GetByModifierGroupIdAsync(modifierGroupId);
        if (modifiersAndGroup == null)
            return null;

        List<int> modifiersIds = modifiersAndGroup.Select(u => u.Modifierid).ToList();
        List<ModifierViewModel> modifierViewModel = new List<ModifierViewModel>();
        foreach (int m in modifiersIds)
        {
            Modifier? modifier = await _modifierRepository.GetModifierById(m);
            if (modifier != null && modifier.Isdeleted == false)
            {
                ModifierViewModel response = new ModifierViewModel()
                {
                    ModifierId = modifier.Modifierid,
                    ModifierName = modifier.Modifiername,
                };
                modifierViewModel.Add(response);
            }
        }

        return new MenuWithItemsViewModel
        {
            Modifiergroupid = modifierGroupId,
            Modifiergroupname = modifiergroup?.Modifiergroupname,
            Modifiergroupdescription = modifiergroup?.Modifiergroupdescription,
            selectedModifiersViewModels = modifierViewModel,
        };
    }

    public async Task<bool> UpdateModifierGroupService(MenuWithItemsViewModel model)
    {
        try{
            if(model.Modifiergroupname!=null)
            {
                List<Modifiergroup>? checkModifierGroup = await _modifierGroupRepository.CheckModifierGroupByName(model.Modifiergroupname.Trim().ToLower());
                if(checkModifierGroup != null && checkModifierGroup.Count > 0 )
                {
                    foreach(Modifiergroup c in checkModifierGroup)
                    {
                        if (c.Modifiergroupid != model.Modifiergroupid && 
                            model.Modifiergroupname != null && 
                            model.Modifiergroupname.Trim().ToLower().Equals(c.Modifiergroupname?.Trim().ToLower()))
                        {
                            return false;
                        }
                    }
                }

            }
            
            
            List<Modfierandgroupsmapping>? modifiersAndGroup =
                await _modifierGroupRepository.GetByModifierGroupIdAsync(model.Modifiergroupid);
            Modifiergroup? modifierGroup = await _modifierGroupRepository.GetModifierGroupById(model.Modifiergroupid);
            if (modifierGroup == null)
                throw new Exception("Modifier group not found");

            // Update basic properties
            modifierGroup.Modifiergroupname = model.Modifiergroupname;
            modifierGroup.Modifiergroupdescription = model.Modifiergroupdescription;
            await _modifierGroupRepository.UpdateModifierGroup(modifierGroup);

            // Update modifier associations
            List<int> existingModifierIds =
                modifiersAndGroup != null
                    ? modifiersAndGroup.Select(m => m.Modifierid).ToList()
                    : new List<int>();
            List<int> newModifierIds = model.SelectedModifierIds ?? new List<int>();

            // Remove modifiers that are no longer selected
            List<int> modifiersToRemove = existingModifierIds.Except(newModifierIds).ToList();
            foreach (int m in modifiersToRemove)
            {
                Modfierandgroupsmapping? mapping = await _modifierGroupRepository.GetMappingsById(
                    model.Modifiergroupid,
                    m
                );
                if (mapping != null)
                    await _modifierGroupRepository.DeleteModfierandGroupsMapping(mapping);
            }

            // Add new modifiers
            List<int> modifiersToAdd = newModifierIds.Except(existingModifierIds).ToList();

            foreach (int m in modifiersToAdd)
            {
                Modfierandgroupsmapping mapping = new Modfierandgroupsmapping()
                {
                    Modifiergroupid = model.Modifiergroupid,
                    Modifierid = m,
                    Createdat = DateTime.Now,
                    Createdbyid = model.Userid != 0 ? model.Userid : 0,
                    Editedat = DateTime.Now,
                    Editedbyid = model.Userid != 0 ? model.Userid : 0,
                };
                await _modifierGroupRepository.UpdateModfierandGroupsMapping(mapping);
            }

            return true;
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw new Exception("Error,code 500: internal server error");
        }
    }

    public async Task<List<ModifierViewModel>> GetModifiersByIds(List<int> modifierIds)
    {
        List<ModifierViewModel>? result = new List<ModifierViewModel>();
        foreach (int modifierId in modifierIds)
        {
            Modifier? modifier = await _modifierRepository.GetModifierById(modifierId);
            if (modifier != null)
            {
                ModifierViewModel modifierViewModel = new ModifierViewModel()
                {
                    ModifierId = modifier.Modifierid,
                    ModifierName = modifier.Modifiername,
                };
                result.Add(modifierViewModel);
            }
        }

        return result;
    }

    public async Task<List<Item>?> GetItemsByCategoryId(
        int? category = null,
        string? searchTerm = null
    )
    {
        List<Item>? items;
        if (category != 0 && category != null)
        {
            items = await _itemRepository.GetItemsByCategoryId(category);
        }
        else if (category == 0)
        {
            items = await _itemRepository.GetAllFavouriteItems();
        }
        else
        {
            items = await _itemRepository.GetAllItems();
        }
        items = items.Where(i => i.Isavailable == true).ToList();
        // search term exists
        if (!string.IsNullOrEmpty(searchTerm))
        {
            items = items.Where(i => i.Itemname.ToLower().Contains(searchTerm.ToLower())).ToList();
        }

        return items.OrderBy(o=>o.Itemid).ToList();
    }
}
