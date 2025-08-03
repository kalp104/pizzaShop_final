using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class UpdateUserViewModel
{
    [Required(ErrorMessage = "firstname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")] ///^[a-zA-Z ]*$/
    [RegularExpression(@"^[a-zA-Z]$", ErrorMessage = "name dosen't contain special case")]
    public string? Firstname { get; set; }

    [Required(ErrorMessage = "lastname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")]
    [RegularExpression(@"^[a-zA-Z]$", ErrorMessage = "name dosen't contain special case")]
    public string? Lastname { get; set; }

    [Required(ErrorMessage = "username is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number, should be exactly 10 digits.")]
    public Decimal Phone { get; set; }

    [Required(ErrorMessage = "address is required")]
    [MaxLength(200, ErrorMessage = "limit exceed ")]
    public string Address { get; set; }
    
    [Required(ErrorMessage = "zipcode is required")]
    [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid Zipcode, should be exactly 6 digits.")]
    public decimal Zipcode { get; set; }
    
    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int cityId { get; set; }
    
    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int countryId { get; set; }
    
    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int stateId { get; set; }

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