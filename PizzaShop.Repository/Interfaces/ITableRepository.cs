using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ITableRepository
{
    Task<List<Table>?> GetTableBySectionIdAsync(int? sectionid);
    Task<List<Table>?> GetAllTables();
    Task<Table?> GetTablesById(int tableid);
    Task<string> UpdateTable(Table table);
    Task<string> AddTable(Table table);
    Task<List<Table>?> CheckTableByName(string name);
    Task<OrdersTablesMapping?> GetOrderByTableId(int tableid);
}
