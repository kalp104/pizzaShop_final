using System;

namespace PizzaShop.Repository.ModelView;

public class OrderDataViewModel
{
    public int OrderId {get;set;}
    public int TableId {get; set;}
    public int CustomerId {get; set;}
    public int PaymentMode {get; set;}
    public decimal TotalAmount {get; set;}
    public string? OrderWiseComment {get; set;}
    public List<OrderItem>? Items { get; set; }
    public List<OrderTax>? Taxes { get; set; }
}

    public class OrderItem
    {
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public decimal ItemRate { get; set; }
        public int TotalItems { get; set; }
        public string? Comment { get; set; }
        public List<OrderModifier>? Modifiers { get; set; }
    }

    public class OrderModifier
    {
        public int ModifierId { get; set; }
        public string? ModifierName { get; set; }
        public decimal ModifierRate { get; set; }
    }

    public class OrderTax
    {

        public string? TaxName { get; set; }
        public int TaxId { get; set; }
        public decimal TaxAmount { get; set; }
        public int TaxType { get; set; }
        public decimal TaxValue { get; set; }
         public bool? Isenabled { get; set; }
         public bool? isChecked {get; set;}
    }