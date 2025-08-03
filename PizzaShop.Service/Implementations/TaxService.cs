using System;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class TaxService : ITaxService
{
    private readonly IGenericRepository<TaxAndFee> _tax;
    private readonly ITaxRepository _taxRepository;

    public TaxService(
        IGenericRepository<TaxAndFee> tax,
        ITaxRepository taxRepository)
    {
        _tax = tax;
        _taxRepository = taxRepository;
    }

    // index page get all tax service
    public async Task<TaxHelperViewModel> GetTaxesAsync(int? taxId = null, string? searchTax = null)
    {
        List<TaxAndFee>? tax = await _taxRepository.GetAllTax();
    
        if (!string.IsNullOrEmpty(searchTax) && tax!=null)
        {
            searchTax = searchTax.ToLower();
            tax = tax.Where(u => u.Taxname.ToLower().Contains(searchTax)).ToList();
        }

        TaxHelperViewModel taxHelper = new TaxHelperViewModel();
        taxHelper.TaxHelpers = tax;
        taxHelper.Taxid = taxId ?? 0;
        return taxHelper;
    }

    public async Task<bool> AddTaxAsync(TaxHelperViewModel newTax, int userid)
    {
        List<TaxAndFee>? checkTax = await _taxRepository.CheckTaxByName(newTax.Taxname.ToLower());
        if(checkTax!=null && checkTax.Count>0){
            return false;
        }
        
        if (newTax != null && userid != 0)
        {
            TaxAndFee tax = new TaxAndFee();
            tax.Taxname = newTax.Taxname;
            tax.Taxamount = newTax.Taxamount;
            tax.Isdefault = newTax.Isdefault;
            tax.Taxtype = newTax.Taxtype;
            tax.Isenabled = newTax.Isenabled;
            tax.Isdeleted = false;
            tax.Createdat = DateTime.Now;
            tax.Createdbyid = userid;
            tax.Editedat = DateTime.Now;
            tax.Editedbyid = userid;
            await _taxRepository.AddTaxes(tax);
        }

        return true;
    }

    public async Task<bool> DeleteTaxAsync(int taxId, int userid)
    {
        try{
            TaxAndFee? tax = await _taxRepository.GetTaxByTaxId(taxId);
            if (tax != null)
            {
                tax.Isdeleted = true;
                tax.Editedat = DateTime.Now;
                tax.Editedbyid = userid;
                tax.Deletedbyid = userid;
                tax.Deletedat = DateTime.Now;
                await _taxRepository.UpdateTaxes(tax);
            }
            return true;
        }catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
        
    }

    public async Task<bool> EditTaxAsync(TaxHelperViewModel model, int userid)
    {
        try{
            List<TaxAndFee>? checkTax = await _taxRepository.CheckTaxByName(model.Taxname.ToLower());
            if(checkTax!=null && checkTax.Count>0){
                foreach(TaxAndFee t in checkTax)
                {
                    if(t.Taxid != model.Taxid && t.Taxname.ToLower().Equals(model.Taxname.ToLower()))
                    {
                        return false;
                    }
                }
            }
            
            TaxAndFee? tax = await _taxRepository.GetTaxByTaxId(model.Taxid);
            if (tax != null)
            {
                tax.Taxname = model.Taxname;
                tax.Taxamount = model.Taxamount;
                tax.Isdefault = model.Isdefault;
                tax.Taxtype = model.Taxtype;
                tax.Isenabled = model.Isenabled;
                tax.Isdeleted = false;
                tax.Editedat = DateTime.Now;
                tax.Editedbyid = userid;
                await _taxRepository.UpdateTaxes(tax);
            }

            return true;
        }catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return false;
        }
        
    }
}
