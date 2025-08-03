using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class TaxRepository : ITaxRepository
{
    private readonly PizzaShop2Context _context;

    public TaxRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<TaxAndFee>?> GetAllTax()
    {
        return await _context.TaxAndFees
                     .Where(x => x.Isdeleted == false)
                     .OrderBy(x => x.Taxid)
                     .ToListAsync();
    }

    public async Task<List<TaxAndFee>?> CheckTaxByName(string name)
    {
        return await _context.TaxAndFees.Where(f => f.Taxname.ToLower().Equals(name) && f.Isdeleted == false).ToListAsync();
    }

    public async Task AddTaxes(TaxAndFee tax)
    {   
        _context.Add(tax);
        await _context.SaveChangesAsync();
    }

    public async Task<TaxAndFee?> GetTaxByTaxId(int taxid)
    {
        return await _context.TaxAndFees
                     .Where(x => x.Taxid == taxid)
                     .FirstOrDefaultAsync();
    }

    public async Task UpdateTaxes(TaxAndFee tax)
    {   
        _context.Update(tax);
        await _context.SaveChangesAsync();
    }
}
