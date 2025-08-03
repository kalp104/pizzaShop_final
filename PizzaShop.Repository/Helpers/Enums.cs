using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Repository.Helpers;

public class Enums
{
    public enum RolesEnum
    {
        AccountManager = 1,
        Chef = 2,
        Admin = 3,
    }

    public enum TaxTypes
    {
        Percentage = 1,
        Fixed = 2,
    }

    public enum KOTStatus
    {
        InProgress = 1,
        Ready = 2,
        CompleteOrCancel = 3
    }
    public enum OrderStatus
    {
        Pending = 1,
        Failed = 2,
        [Description("In Progress")]
        [Display(Name ="In Progress")]
        InProgress = 3,
        Served = 4,
        Completed = 5,
        Cancelled = 6,
        [Display(Name = "On Hold")]
        OnHold = 7,
        [Display(Name = "All Status")]
        AllStatus = 8,
    }

    public enum EventStatus
    {
        pending = 1,
        Expired = 2,
        Complete = 3,
        Cancelled = 4
    }

    public enum EventType
    {
        Dinein = 1,
        [Display(Name ="Take away")]
        TakeAway = 2
    }

    public enum Ordertype
    {
        Dinein = 1,
        parcel = 2
    }

    public enum TableStatus
    {
        Available = 1,
        Assigned = 2,
        Running = 3,
    }

    public enum TableStatusAtSection
    {
        Available = 1,
        Assigned = 2,
        
    }
    public enum DateRange
    {   
        [Display(Name = "All Time")]
        AllTime = 1,
        [Display(Name = "Last 7 days")]
        Last7days = 2,
        [Display(Name = "Last 30 days")]
        Last30days = 3,
        [Display(Name = "Current Month")]
        CurrentMonth = 4,
    }

    public enum DateRangeCustom
    {
        [Display(Name = "All Time")]
        AllTime = 1,
        [Display(Name = "Last 7 days")]
        Last7days = 2,
        [Display(Name = "Last 30 days")]
        Last30days = 3,
        [Display(Name = "Current Month")]
        CurrentMonth = 4,
        [Display(Name = "Custom Date")]
        CustomDate = 5
    }

    public enum PaymentMethod
    {
        None = 0,
        Cash = 1,
        UPI = 3,
        Card = 2,
    }

    public enum Parameter
    {
        Gram = 1,
        Kg = 2,
        Ml = 4,
        Lit = 3,
        Pieces = 5
    }

}
