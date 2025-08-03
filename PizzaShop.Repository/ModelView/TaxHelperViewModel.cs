using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PizzaShop.Repository.Models;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Repository.ModelView;

public class TaxHelperViewModel
{
    public int Taxid { get; set; }

    [Required(ErrorMessage = "Tax name is required")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only Alphabets and Numbers are allowed.")]
    public string Taxname { get; set; } = null!;

    [Required(ErrorMessage = "Tax type is required")]
    public int Taxtype { get; set; }

    [Required(ErrorMessage = "Tax amount is required")]
    [Column(TypeName = "decimal(18,2)")]
    [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
    public decimal Taxamount { get; set; }
    public bool Isenabled { get; set; } = false;

    public bool Isdefault { get; set; } = false;

    public List<TaxAndFee>? TaxHelpers { get; set; }

    public TaxTypes TaxTypes { get; set; }
}

public class TaxHelper
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public int Taxtype { get; set; }

    public decimal Taxamount { get; set; }

    public bool? Isenabled { get; set; }

    public bool? Isdefault { get; set; }

}