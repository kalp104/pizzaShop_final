using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzaShop2Context _context;
    public ItemRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<Item?> GetItemById(int itemid)
    {
        return await _context.Items
                     .Where(x => x.Itemid == itemid)
                     .FirstOrDefaultAsync();
    }

    public async Task<List<Item>> GetItemsByCategoryId(int? categoryid)
    {
        return await _context
                .Items.Where(u => u.Isdeleted == false && u.Categoryid == categoryid)
                .OrderBy(u => u.Itemid)
                .ToListAsync();
    }

    public async Task<List<Item>> GetAllItems()
    {
        return await _context
                .Items.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Itemid)
                .ToListAsync();
    }

    public async Task<List<Item>> GetAllFavouriteItems()
    {
        return await _context
                .Items.Where(u => u.Isdeleted == false && u.Favourite == true)
                .OrderBy(u => u.Itemid)
                .ToListAsync();
    }

    public async Task UpdateItem(Item item)
    {   
        _context.Update(item);
        await _context.SaveChangesAsync();
    }


    public async Task AddItem(Item item)
    {   
        _context.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Item>?> CheckItemByName(string name)
    {
        return await _context.Items.Where(s => s.Itemname != null && s.Itemname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
    }

}
