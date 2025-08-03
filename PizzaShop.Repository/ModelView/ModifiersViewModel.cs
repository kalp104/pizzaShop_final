using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class ModifiersViewModel
{
    public int Modifierid { get; set; }
    
    [Required(ErrorMessage = "please select the modifier group")]
    public List<int>? Modifiergroupid { get; set; }

    [Required(ErrorMessage = "name is required")]
    [StringLength(50, ErrorMessage = "name cannot exceed 50 characters")]
    public string? Modifiername { get; set; }
    
    [Required(ErrorMessage = "Rate is required")]
    public decimal? Modifierrate { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal? Modifierquantity { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid unit")]
    public decimal? Modifierunit { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Modifierdescription { get; set; }

}
