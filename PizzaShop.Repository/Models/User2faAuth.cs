using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class User2faAuth
{
    public int Userid { get; set; }

    public string TokenAuth { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Rememberme { get; set; }

    public DateTime ExpireTime { get; set; }

    public DateTime CreateTime { get; set; }

    public int Counting { get; set; }
}
