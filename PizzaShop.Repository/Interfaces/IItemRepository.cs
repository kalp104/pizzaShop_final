using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetItemById(int itemid);
    Task<List<Item>> GetItemsByCategoryId(int? categoryid);
    Task<List<Item>> GetAllItems();
    Task UpdateItem(Item item);
    Task AddItem(Item item);

    Task<List<Item>?> CheckItemByName(string name);

    Task<List<Item>> GetAllFavouriteItems();
}
