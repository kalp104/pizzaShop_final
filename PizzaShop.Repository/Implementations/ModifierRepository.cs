using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class ModifierRepository : IModifierRepository
{
    private readonly PizzaShop2Context _context;
    public ModifierRepository(PizzaShop2Context context)
    {
        _context = context;       
    }


    public async Task<Modifier?> GetModifierById(int modifierid)
    {
        return await _context.Modifiers
                     .Where(x => x.Modifierid == modifierid)
                     .FirstOrDefaultAsync();
    }

    public async Task<List<Modifier>> GetAllModifiers()
    {
        return await _context
                .Modifiers
                .Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Modifierid)
                .ToListAsync();
    }

    public async Task<List<Modifier>> GetModifiersByMG(int? id)
    {
            // return await _context.Modifiers.Where(u => u.Isdeleted == false && u.Modifiergroupid == id ).OrderBy(u => u.Modifierid).ToListAsync();

            var result = await (
                from mgm in _context.Modfierandgroupsmappings
                join m in _context.Modifiers on mgm.Modifierid equals m.Modifierid
                where mgm.Modifiergroupid == id
                where m.Isdeleted == false
                orderby m.Modifierid
                select new Modifier
                {
                    Modifierid = m.Modifierid,
                    Modifiername = m.Modifiername,
                    Modifierrate = m.Modifierrate,
                    Modifierquantity = m.Modifierquantity,
                    Modifierunit = m.Modifierunit,
                    Modifierdescription = m.Modifierdescription,
                    Taxpercentage = m.Taxpercentage,
                    Isdeleted = m.Isdeleted,
                    Createdat = m.Createdat,
                    Deletedat = m.Deletedat,
                    Editedat = m.Editedat,
                    Editedbyid = m.Editedbyid,
                    Createdbyid = m.Createdbyid,
                    Deletedbyid = m.Deletedbyid,
                }
            ).ToListAsync();
            return result;
    }


    public async Task UpdateModifier(Modifier modifier)
    {   
        _context.Update(modifier);
        await _context.SaveChangesAsync();
    }


    public async Task AddModifier(Modifier modifier)
    {   
        _context.Add(modifier);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Modifier>?> CheckModifierByName(string name)
    {
        return await _context.Modifiers.Where(s => s.Modifiername != null && s.Modifiername.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
    }

}
