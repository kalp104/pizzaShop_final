using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Event
{
    public int EventId { get; set; }

    public DateTime EventDate { get; set; }

    public int EventType { get; set; }

    public int NumberOfPerson { get; set; }

    public int OrderType { get; set; }

    public int AcSwitch { get; set; }

    public int? EventStatus { get; set; }

    public string? BillingAddress { get; set; }

    public string? SpecialInstruction { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? EditedAt { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CreatedById { get; set; }

    public int? DeletedById { get; set; }

    public int? EditedById { get; set; }

    public int StartEventTime { get; set; }

    public int EndEventTime { get; set; }

    public virtual ICollection<EventCustomerMapping> EventCustomerMappings { get; set; } = new List<EventCustomerMapping>();

    public virtual ICollection<EventSpecialType> EventSpecialTypes { get; set; } = new List<EventSpecialType>();
}
