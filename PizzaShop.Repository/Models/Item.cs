using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public int Categoryid { get; set; }

    public string? Itemname { get; set; }

    public int? Itemtype { get; set; }

    public decimal? Rate { get; set; }

    public decimal? Quantity { get; set; }

    public int? Status { get; set; }

    public int? Shortcode { get; set; }

    public string? Description { get; set; }

    public string? Imageid { get; set; }

    public decimal? Taxpercentage { get; set; }

    public bool? Favourite { get; set; }

    public int? Units { get; set; }

    public DateTime? Editedat { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public bool? Isavailable { get; set; }

    public bool? DefaultTax { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; } = new List<ItemModifiergroupMapping>();

    public virtual ICollection<OrderItemMapping> OrderItemMappings { get; set; } = new List<OrderItemMapping>();
}
