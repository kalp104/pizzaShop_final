using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class City
{
    public int Cityid { get; set; }

    public int Stateid { get; set; }

    public string Cityname { get; set; } = null!;

    public virtual State State { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
