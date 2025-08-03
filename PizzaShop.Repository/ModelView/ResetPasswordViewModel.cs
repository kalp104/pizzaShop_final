using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class ResetPasswordViewModel
{   
    public string? Email { get; set; }
    public string? Token { get; set; }
    
    [Required(ErrorMessage = "Password is Required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Atleast contain 1-uppercase, 1-lowercase, 1-special charecter, 1-number  and length should be 8")]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Password is Required")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }

}
/*
Email validation string :
[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]

Password validation string: 
[RegularExpression(@"^(?=.[a-z])(?=.[A-Z])(?=.\d)(?=.[@#$^+=!()%-])[A-Za-z\d@#$^+=!()%-]{8,}$", ErrorMessage = "please enter strong password")]

Zipcode :
[RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid Zipcode, should be exactly 6 digits.")]

Phone: 
[RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number, should be exactly 10 digits.")]

*/