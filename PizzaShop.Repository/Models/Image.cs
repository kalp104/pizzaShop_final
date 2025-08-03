using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Image
{
    public int Imageid { get; set; }

    public int? Userid { get; set; }

    public string? Imageurl { get; set; }

    public virtual User? User { get; set; }
}
