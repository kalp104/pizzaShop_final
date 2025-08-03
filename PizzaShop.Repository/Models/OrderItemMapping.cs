using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class OrderItemMapping
{
    public int Orderitemmappingid { get; set; }

    public int Orderid { get; set; }

    public int Itemid { get; set; }

    public decimal? Totalquantity { get; set; }

    public string? Specialmessage { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Status { get; set; }

    public int? Readyquantity { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<OrderItemModifiersMapping> OrderItemModifiersMappings { get; set; } = new List<OrderItemModifiersMapping>();
}
