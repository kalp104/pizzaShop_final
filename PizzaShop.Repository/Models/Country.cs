using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Country
{
    public int Countryid { get; set; }

    public string Countryname { get; set; } = null!;

    public virtual ICollection<State> States { get; set; } = new List<State>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
