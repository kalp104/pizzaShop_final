using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class OrderAppMenuViewModel
{
    public List<Category>? categories { get; set; }
    public List<Item>? items {get; set;}
    public CustomerEditViewModel? customer {get; set;}

    public int Totalperson {get;set;}
    public int Tableid {get;set;} 
    public int TableStatus {get;set;}
    public OrderPageViewModel? orderPageViewModel { get; set; } 
}

public class OrderPageViewModel
{
    public int tableId {get; set;}
    public int sectionId {get; set;}
    public string? sectionName {get; set;}
    public int orderId { get; set; }
    public int customerId { get; set; }



    public List<TableHelper>? tableHelpers { get; set; } 
    public List<TaxAndFee>? taxAndFees {get;set;}
}
