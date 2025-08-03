using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class WaitingListRepository : IWaitingListRepository
{
    private readonly PizzaShop2Context _context;

    public WaitingListRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<WaitingList>?> GetWaitingListsByDateFilters(DateTime startDate, DateTime endDate)
    {
        return await _context.WaitingLists
                     .Where(w => w.Createdat >= startDate && w.Createdat <= endDate && w.Isdeleted == false)
                     .ToListAsync();
    }

    public async Task<List<WaitingList>?> GetAllWaitingLists()
    {
        return await _context.WaitingLists
                     .Where(w => w.Isdeleted == false)
                     .ToListAsync();
    }
    public async Task<List<WaitingList>?> GetAllWaitingListsBySectionId(int? sectionid = null)
    {
        if(sectionid == null)
            return await _context.WaitingLists
                     .Where(w => w.Isdeleted == false)
                     .ToListAsync();
        
        else 
            return await _context.WaitingLists
                     .Where(w => w.Isdeleted == false && w.Sectionid == sectionid)
                     .ToListAsync();
    }


    public async Task<string> AddWaitingList(WaitingList waitingList)
    {
        try{
            _context.Add(waitingList);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> UpdateWaitingList(WaitingList waitingList)
    {
        try{
            _context.Update(waitingList);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<WaitingList?> GetWaitingListById(int waitinglist)
    {
        return await _context.WaitingLists
                     .Where(x => x.Waitingid == waitinglist)
                     .FirstOrDefaultAsync();
    }
}
