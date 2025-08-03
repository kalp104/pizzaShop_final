using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class TableRepository : ITableRepository
{   
    private readonly PizzaShop2Context _context;

    public TableRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<Table>?> GetTableBySectionIdAsync(int? sectionid)
    {
        return await _context.Tables
                .Where(u => u.Isdeleted == false && u.Sectionid == sectionid)
                .OrderBy(u => u.Tableid)
                .ToListAsync();
    }


    public async Task<List<Table>?> GetAllTables()
    {
        return await _context.Tables
                .Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Tableid)
                .ToListAsync();
    }

    public async Task<Table?> GetTablesById(int tableid)
    {
        return await _context.Tables
                .Where(u => u.Tableid == tableid)
                .FirstOrDefaultAsync();
    }

    public async Task<string> UpdateTable(Table table)
    {
        try{
            _context.Update(table);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddTable(Table table)
    {
        try{
            _context.Add(table);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

     public async Task<List<Table>?> CheckTableByName(string name)
    {
        return await _context.Tables.Where(s => s.Tablename != null && s.Tablename.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
    }

    public async Task<OrdersTablesMapping?> GetOrderByTableId(int tableid)
    {
        OrdersTablesMapping? result = await (
                from ot in _context.OrdersTablesMappings
                join o in _context.Orders on ot.Orderid equals o.Orderid
                where ot.Tableid == tableid && o.Isdeleted == false
                orderby o.Createdat descending
                select new OrdersTablesMapping
                {
                    Orderid = o.Orderid,
                    Tableid = ot.Tableid
                }
        ).FirstOrDefaultAsync();
        return result;
    }
}
