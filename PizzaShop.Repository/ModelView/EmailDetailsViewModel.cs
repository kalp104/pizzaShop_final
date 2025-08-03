using System;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.ModelView;

public class EmailDetailsViewModel
{
    [Required(ErrorMessage = "Email address is required")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email is not valid")]
    public string? ToEmail { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Message is required")]
    public string? Message { get; set; }
}
