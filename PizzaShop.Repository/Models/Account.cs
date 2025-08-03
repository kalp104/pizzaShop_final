using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.Models;

public partial class Account
{
    public int Accountid { get; set; }

    [Required(ErrorMessage = "email is required")]
    [MaxLength(50, ErrorMessage = "limit exceed ")]
    [RegularExpression(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        ErrorMessage = "Email is not valid"
    )]
    public string Email { get; set; } = null!;

    public int? Createdbyid { get; set; }

    public int? Deletedbyid { get; set; }

    [MaxLength(50, ErrorMessage = "limit exceed ")]
    [Required(ErrorMessage = "password is required")]
    [RegularExpression(
        "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[#$^+=!*()@%&]).{8,}$",
        ErrorMessage = "Enter strong password"
    )]
    public string Password { get; set; } = null!;

    public string? Username { get; set; }

    public bool? Rememberme { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Roleid { get; set; }

    public bool? Isdeleted { get; set; }

    public virtual Role? Role { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
