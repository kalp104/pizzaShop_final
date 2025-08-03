using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class PasswordResetRequest
{
    public Guid Id { get; set; }

    public int Userid { get; set; }

    public DateTime? Createdate { get; set; }

    public DateTime? Closedate { get; set; }

    public string Guidtoken { get; set; } = null!;
}
