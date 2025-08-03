using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ITaxRepository
{
    Task<List<TaxAndFee>?> GetAllTax();
    Task<List<TaxAndFee>?> CheckTaxByName(string name);
    Task AddTaxes(TaxAndFee tax);
    Task<TaxAndFee?> GetTaxByTaxId(int taxid);
    Task UpdateTaxes(TaxAndFee tax);
}
