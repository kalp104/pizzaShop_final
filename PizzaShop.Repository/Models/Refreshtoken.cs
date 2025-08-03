using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public string Useremail { get; set; } = null!;

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public bool Isrevoked { get; set; }
}
