using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Repository.ModelView;

public class UserProfileViewModel
{

    [Required(ErrorMessage = "firstname is required")]
    [Range(2, 40, ErrorMessage = "limit exceed ")]
    public string? firstname { get; set; } = "";

    [Required(ErrorMessage = "lastname is required")]
    [MaxLength(40, ErrorMessage = "limit exceed ")]
    public string? lastname { get; set; } = "";

    [Required(ErrorMessage = "username is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    public string? username { get; set; }

    [Required(ErrorMessage = "zipcode is required")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid Zipcode, should be 6 numbers")]
    public decimal? zipcode { get; set; }
    
    [Required(ErrorMessage = "address is required")]
    public string? address { get; set; }

    [Required(ErrorMessage = "phone number is required")]
    [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
    public decimal? phonenumber { get; set; }

    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int cityid { get; set; }

    [Required(ErrorMessage = "state is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a state")]
    public int stateid { get; set; }

    [Required(ErrorMessage = "country is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a country")]
    public int countryid { get; set; }
    public IFormFile? File { get; set; }
    public int roleid { get; set; }
    public int uid {get; set;}
    public int aid {get; set;}

}


