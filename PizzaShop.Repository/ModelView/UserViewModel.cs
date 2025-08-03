
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Repository.ModelView;

public class UserViewModel
{
    public int Id { get; set; } = 0;
    [Required(ErrorMessage = "firstname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")] ///^[a-zA-Z ]*$/
    [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "invalid input")]
    public string? Firstname { get; set; } ="";

    [Required(ErrorMessage = "lastname is required")]
    [MaxLength(40,ErrorMessage = "limit exceed ")]
    [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "invalid input")]
    public string? Lastname { get; set; } = "";

    [Required(ErrorMessage = "username is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    public string? Username { get; set; }
    public string? Rolename { get; set; } = "";

    [Required(ErrorMessage = "email is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
    public string? Email { get; set; } = "";

    [Required(ErrorMessage = "status is required")]
    public int? Status { get; set; } = 3;

    public string? cityname { get; set; } = "";

    public string? Countryname { get; set; } = "";

    public string? Statename { get; set; } = "";

    [Required(ErrorMessage = "zipcode is required")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid Zipcode, should be 6 numbers")]
    public decimal? Zipcode { get; set; }
    [Required(ErrorMessage = "address is required")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "phone number is required")]
    [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
    public decimal? phone { get; set; }

    [Required(ErrorMessage = "country is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a country")]
    public int countryId { get; set; }

    [Required(ErrorMessage = "state is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a state")]
    public int stateId { get; set; }

    [Required(ErrorMessage = "city is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Select a city")]
    public int cityId { get; set; }
    
    public int accountId { get; set; }


    public IFormFile? imageFile { get; set; } 
    public string? ImageUrl { get; set; }


    public int roleId { get; set; }
    // public string? rolename { get; set; }
    public int userId { get; set; }
    // Add a list of countries
    public List<Country1>? Countries { get; set; } = new List<Country1> {};

}


public class Country1
{
    public int CountryId { get; set; }
    public string? CountryName { get; set; }
}

