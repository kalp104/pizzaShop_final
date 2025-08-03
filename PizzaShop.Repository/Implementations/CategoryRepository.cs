using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly PizzaShop2Context _context;

    public CategoryRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<Category>> GetAllCategories()
    {
        return await _context
                .Categories.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Categoryid)
                .ToListAsync();
    }

    public async Task<Category?> GetCategoryById(int categoryid)
    {
        return await _context
                .Categories.Where(u => u.Categoryid == categoryid)
                .FirstOrDefaultAsync();
    }

    public async Task<List<Category>?> CheckCategoryByName(string name)
    {
        return await _context.Categories.Where(s => s.Categoryname != null && s.Categoryname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
    }


    public async Task UpdateCategory(Category category)
    {   
        _context.Update(category);
        await _context.SaveChangesAsync();
    }


    public async Task AddCategory(Category category)
    {   
        _context.Add(category);
        await _context.SaveChangesAsync();
    }

}
