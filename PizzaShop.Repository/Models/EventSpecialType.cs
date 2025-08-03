using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class EventSpecialType
{
    public int SpecialTypeId { get; set; }

    public int EventId { get; set; }

    public string? EventTypeDetails { get; set; }

    public virtual Event Event { get; set; } = null!;
}
