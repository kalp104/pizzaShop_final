using System;
using System.ComponentModel.DataAnnotations;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class OrderAppTableViewModel
{
    public List<WaitingListViewModel>? waitingList { get; set; }
    public List<SectionsOrderAppViewModel>? sections { get; set; }

    public int Waitingid { get; set; }

    [Required(ErrorMessage = "Section is required")]
    public int Sectionid { get; set; }

    public int Tableid { get; set; }

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
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Invalid input")]
    public int? TotalPersons { get; set; }

    public string? tableIds { get; set; }

    public List<int> Tableids { get; set; } = new List<int>();
}

public class SectionsOrderAppViewModel
{
    public int Sectionid { get; set; }

    public string Sectionname { get; set; } = null!;

    public string? Sectiondescription { get; set; }

    public int Available { get; set; }
    public int Running { get; set; }
    public int Assigned { get; set; }
    public List<TableOrderAppViewModel>? tables { get; set; }
}

public class TableOrderAppViewModel
{
    public decimal Amount { get; set; }
    public string? OrderTime { get; set; }
    public int Tableid { get; set; }
    public string Tablename { get; set; } = null!;
    public int? Status { get; set; }
    public int Sectionid { get; set; }
    public int? TCapacity { get; set; }
}

public class WaitingListViewModel
{
    public int Waitingid { get; set; }
    public string Customername { get; set; } = null!;
    public string Customeremail { get; set; } = null!;
    public decimal? Customerphone { get; set; }
    public int? TotalPersons { get; set; }
    public int Sectionid { get; set; }
    public string? Sectionname { get; set; }
    public DateTime? createdAt { get; set; }
}
