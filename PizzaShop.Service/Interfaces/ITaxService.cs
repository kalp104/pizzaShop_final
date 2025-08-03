using System;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface ITaxService
{
    Task<TaxHelperViewModel> GetTaxesAsync(int? taxId = null, string? searchTax = null);

    Task<bool> AddTaxAsync(TaxHelperViewModel newTax, int userid);

    Task<bool> DeleteTaxAsync(int taxId, int userid);

    Task<bool> EditTaxAsync(TaxHelperViewModel model, int userid);
}
