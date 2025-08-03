using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class State
{
    public int Stateid { get; set; }

    public int Countryid { get; set; }

    public string Statename { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
