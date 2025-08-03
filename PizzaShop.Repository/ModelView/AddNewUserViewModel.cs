using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Repository.ModelView;

public class AddNewUserViewModel
{   
    [Required(ErrorMessage = "firstname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")] ///^[a-zA-Z ]*$/
    [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "invalid input")]
    public string Firstname { get; set; } = string.Empty;

    [Required(ErrorMessage = "lastname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")]
    [RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "name dosen't contain special case")]
    public string? Lastname { get; set; }
    
    [Required(ErrorMessage = "username is required")]
    [MaxLength(50,ErrorMessage = "limit exceed ")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "role is required")]
    public int roleId { get; set; }

    [Required(ErrorMessage = "email is required")]
    [MaxLength(50,ErrorMessage = "limit exceed ")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
    public string? Email { get; set; }
    [MaxLength(50,ErrorMessage = "limit exceed ")]
    [Required(ErrorMessage = "password is required")]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{8,}$", ErrorMessage = "Atleast contain 1-uppercase, 1-lowercase, 1-special charecter, 1-number  and length should be 8")]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "zipcode is required")]
    [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid Zipcode, should be exactly 6 digits.")]
    public decimal Zipcode { get; set; }

    [Required(ErrorMessage = "address is required")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Invalid phone number, should be exactly 10 digits.")]
    public decimal phone { get; set; }

    [Required(ErrorMessage = "country is required")]
     [Range(1, int.MaxValue, ErrorMessage = "Select a country")]
    public int countryId { get; set; }

    [Required(ErrorMessage = "state is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a state")]
    public int stateId { get; set; }

    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int cityId { get; set; }
    
    [Required(ErrorMessage = "image is required")]
    public IFormFile? imageFile { get; set; }

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