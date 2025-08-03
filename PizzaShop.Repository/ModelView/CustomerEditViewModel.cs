using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class CustomerEditViewModel
{
    
    public int Customerid { get; set; }
    public int Orderid {get;set;}
    public int Tableid {get;set;}

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(100, ErrorMessage = "limit exceed ")]
    public string Customername { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    [RegularExpression(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Email is not valid"
    )]
    public string Customeremail { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number, should be exactly 10 digits.")]
    public decimal Customerphone { get; set; }

    [Required(ErrorMessage = "Total person is required")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid number")]
    public int Totalperson {get;set;}

}
