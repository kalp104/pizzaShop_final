using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllCategories();
    Task<List<Category>?> CheckCategoryByName(string name);

    Task UpdateCategory(Category category);
    Task AddCategory(Category category);

    Task<Category?> GetCategoryById(int categoryid);
}
