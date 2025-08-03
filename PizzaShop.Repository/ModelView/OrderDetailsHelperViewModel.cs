using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PizzaShop.Repository.ModelView;

public class OrderDetailsHelperViewModel
{
    //order details
    public int Orderid { get; set; }

    public int Status { get; set; }

    public int Paymentmode { get; set; }

    public decimal? Ratings { get; set; }

    public decimal Totalamount { get; set; }

    public string? Orderdescription { get; set; }

    public DateTime? Createdat { get; set; }
    public string? completedAtString { get; set; }
    public DateTime? CompletedAt { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Totalpersons { get; set; }


    //customer details
    public int Customerid { get; set; }
    public string Customername { get; set; } = null!;
    public string Customeremail { get; set; } = null!;
    public decimal Customerphone { get; set; }


    //section and table details 

    public decimal SubTotal {get;set;}
    public decimal Total {get;set;}

    public List<Repository.Models.Table>? Tables { get; set; }
    public int Sectionid { get; set; }
    public string Sectionname { get; set; } = null!;

    public List<ItemOrderViewModel>? ItemAtOrder { get; set; }
    
    public List<TaxAmountViewModel>? TaxAmount {get;set;}


    // feed back details
    public string? FeedbackComment { get; set; }
    public int foodRating { get; set; }
    public int serviceRating { get; set; }
    public int ambienceRating { get; set; }
}


public class ItemOrderViewModel
{
    public int itemId { get; set; }
    public string? itemName { get; set; }
    public decimal? Rate { get; set; }

    public int OrderItemMappingId { get; set; }
    public decimal? totalQuantity { get; set; }
    public List<ModifiersOrderViewModel>? ModifierOrder { get; set; }
}

public class ModifiersOrderViewModel
{
    public int modifierId { get; set; }
    public string? modifierName { get; set; }
    public decimal? modifierRate {get; set;}
}


public class TaxAmountViewModel {

    public int TaxId {get;set;}
    public string? TaxName {get;set;}
    public decimal TaxAmount {get;set;} 
}