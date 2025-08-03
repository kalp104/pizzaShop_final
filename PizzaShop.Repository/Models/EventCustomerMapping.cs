using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class EventCustomerMapping
{
    public int MappingId { get; set; }

    public int Customerid { get; set; }

    public int EventId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? CreatedById { get; set; }

    public int? EditedById { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
