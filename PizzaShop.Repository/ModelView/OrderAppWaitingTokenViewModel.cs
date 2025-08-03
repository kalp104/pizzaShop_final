using System.ComponentModel.DataAnnotations;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class OrderAppWaitingTokenViewModel
{
    public List<SectionWatitingTokenViewModel>? sections { get; set; }
    public List<Table>? tables { get; set; }
    public List<WaitingListViewModel>? waitingLists { get; set; }
    public int Waitingid { get; set; }

    [Required(ErrorMessage = "Table is required")]
    public int Tableid { get; set; } 

    [Required(ErrorMessage = "Section is required")]
    public int Sectionid { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [RegularExpression("^[a-zA-Z_  ]*$", ErrorMessage = "invalid input")]
    public string Customername { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [MaxLength(100, ErrorMessage = "limit exceed ")]
    [RegularExpression(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Invalid user credentials"
    )]
    public string Customeremail { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(
        @"^[0-9]{10}$",
        ErrorMessage = "Invalid phone number, should be exactly 10 digits."
    )]
    public decimal? Customerphone { get; set; }

    [Required(ErrorMessage = "Number of Persons required")]
    [Range(1, 10000, ErrorMessage = "Number of persons should be between 1 and 100")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid input")]
    public int? TotalPersons { get; set; }

    public List<Customer>? customers { get; set; }

    public bool found { get; set; } = false;
}
