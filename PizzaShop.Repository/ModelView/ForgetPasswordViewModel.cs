using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class ForgetPasswordViewModel
{
    public string? Email { get; set; }
    public string? Token { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Atleast contain 1-uppercase, 1-lowercase, 1-special charecter, 1-number  and length should be 8")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string? ConfirmPassword { get; set; }
}




