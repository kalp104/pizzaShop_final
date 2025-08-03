using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class ModifierGroupRepository : IModifierGroupRepository
{
    private readonly PizzaShop2Context _context;

    public ModifierGroupRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroups()
    {
        return await _context
                .Modifiergroups
                .Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Modifiergroupid)
                .ToListAsync();
    }
    public async Task<List<Modfierandgroupsmapping>> GetAllModfierandgroupsmapping()
    {
        return await _context
                .Modfierandgroupsmappings
                .OrderBy(u => u.Modfierandgroupsmappingid)
                .ToListAsync();
    }

    public async Task<Modifiergroup?> GetModifierGroupById(int mgid)
    {
        return await _context
                .Modifiergroups.Where(u => u.Modifiergroupid == mgid)
                .FirstOrDefaultAsync();
    }


    public async Task UpdateModifierGroup(Modifiergroup MG)
    {   
        _context.Update(MG);
        await _context.SaveChangesAsync();
    }


    public async Task AddModifierGroup(Modifiergroup MG)
    {   
        _context.Add(MG);
        await _context.SaveChangesAsync();
    }

    public async Task AddModfierandGroupsMapping(Modfierandgroupsmapping mapping)
    {   
        _context.Add(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteModfierandGroupsMapping(Modfierandgroupsmapping mapping)
    {   
        _context.Remove(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateModfierandGroupsMapping(Modfierandgroupsmapping mapping)
    {   
        _context.Update(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Modifiergroup>?> CheckModifierGroupByName(string name)
    {
        return await _context.Modifiergroups
                    .Where(s => s.Modifiergroupname != null && s.Modifiergroupname.Trim().ToLower().Equals(name) && s.Isdeleted == false)
                    .ToListAsync();
    }

    public async Task<Modfierandgroupsmapping?> GetMappingsById(int Modifiergroupid,int modifierid)
    {
        return await _context
                .Modfierandgroupsmappings.Where(u =>
                    u.Modifierid == modifierid && u.Modifiergroupid == Modifiergroupid
                )
                .FirstOrDefaultAsync();
    }


    public async Task<List<ItemModifiergroupMapping>?> GetByModifierGroupIdMappingAsync(int? modifierGroupId)
    {
        return await _context
                .ItemModifiergroupMappings.Where(u => u.Modifiergroupid == modifierGroupId)
                .ToListAsync();
    }


    public async Task DeleteItemModifierGroupMapping(ItemModifiergroupMapping mapping)
    {   
        _context.Remove(mapping);
        await _context.SaveChangesAsync();
    }
    public async Task AddItemModifierGroupMapping(ItemModifiergroupMapping mapping)
    {   
        _context.Add(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemModifierGroupMapping(ItemModifiergroupMapping mapping)
    {   
        _context.Update(mapping);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Modfierandgroupsmapping>?> GetByModifierIdAsync(int? modifiersid)
    {
        return await _context
                .Modfierandgroupsmappings.Where(u => u.Modifierid == modifiersid)
                .ToListAsync();
    }

    public async Task<List<ItemModifiergroupMapping>?> GetByItemIdAsync(int? itemId)
    {
        return await _context
                .ItemModifiergroupMappings.Where(u => u.Itemid == itemId && u.Isdeleted == false)
                .ToListAsync();
    }

    public async Task<List<Modifier>> GetModifiersByMGAsync(int? id)
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


    public async Task<List<Modfierandgroupsmapping>?> GetByModifierGroupIdAsync(int? modifierGroupId)
    {
        return await _context
                .Modfierandgroupsmappings.Where(u => u.Modifiergroupid == modifierGroupId)
                .ToListAsync();
    }
}
