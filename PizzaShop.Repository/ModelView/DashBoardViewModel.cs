using System;

namespace PizzaShop.Repository.ModelView;

public class DashBoardViewModel
{   
    public int TotalOrders { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AvgOrderValue { get; set; }
    public string? AvgWaitingTime {get;set;}
    public int TotalWaittingList {get;set;}
    public int TotalNewCustomer {get;set;}
    public List<ItemWithCount>? topItems {get;set;}
    public List<ItemWithCount>? LastItems {get;set;}
    public List<GraphCustomerViewModel>? GraphDataCustomer {get;set;}

    public List<GraphRevenueViewModel>? graphDataRevenue {get;set;}


}

public class DifferenceWaitingTime {
    public DateTime? EndTime {get;set;}
    public DateTime? StartTime {get;set;}
}


public class ItemWithCount {
    public string? ItemName {get;set;}
    public int Count {get;set;}
    public string? Image {get;set;}
}

public class GraphRevenueViewModel{
    public decimal revenue {get;set;}
    public DateTime date {get;set;}
    public string dateNumber {get;set;} = "";
}

public class GraphCustomerViewModel{
    public int NumberOfCustomer {get;set;}
    public string? Month {get;set;} = "";
    public DateTime Date {get;set;}
}