using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Category
{
    public int Categoryid { get; set; }

    public string Categoryname { get; set; } = null!;

    public string? Categorydescription { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
