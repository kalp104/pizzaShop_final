using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface IModifierRepository
{
    Task<Modifier?> GetModifierById(int modifierid);
    Task<List<Modifier>> GetModifiersByMG(int? id);
    Task<List<Modifier>> GetAllModifiers();
    Task AddModifier(Modifier modifier);
    Task UpdateModifier(Modifier modifier);

    Task<List<Modifier>?> CheckModifierByName(string name);

    
}
