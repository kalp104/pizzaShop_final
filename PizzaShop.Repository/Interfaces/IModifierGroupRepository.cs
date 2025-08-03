using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface IModifierGroupRepository
{
    Task<List<Modifiergroup>> GetAllModifierGroups();
    Task UpdateModifierGroup(Modifiergroup MG);
    Task AddModifierGroup(Modifiergroup MG);
    Task AddModfierandGroupsMapping(Modfierandgroupsmapping mapping);
    Task DeleteModfierandGroupsMapping(Modfierandgroupsmapping mapping);
    Task UpdateModfierandGroupsMapping(Modfierandgroupsmapping mapping);
    Task<List<Modifiergroup>?> CheckModifierGroupByName(string name);
    Task<Modfierandgroupsmapping?> GetMappingsById(int Modifiergroupid,int modifierid);

    Task<Modifiergroup?> GetModifierGroupById(int mgid);
    Task<List<Modfierandgroupsmapping>> GetAllModfierandgroupsmapping();

    Task<List<ItemModifiergroupMapping>?> GetByModifierGroupIdMappingAsync(int? modifierGroupId);

    Task DeleteItemModifierGroupMapping(ItemModifiergroupMapping mapping);
    Task AddItemModifierGroupMapping(ItemModifiergroupMapping mapping);
    Task UpdateItemModifierGroupMapping(ItemModifiergroupMapping mapping);

    Task<List<Modfierandgroupsmapping>?> GetByModifierIdAsync(int? modifiersid);
    Task<List<ItemModifiergroupMapping>?> GetByItemIdAsync(int? itemId);
    Task<List<Modifier>> GetModifiersByMGAsync(int? id);

    Task<List<Modfierandgroupsmapping>?> GetByModifierGroupIdAsync(int? modifierGroupId);




}
