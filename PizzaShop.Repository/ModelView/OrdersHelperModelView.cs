using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class OrdersHelperModelView
{
    public List<OrderCutstomerViewModel>? orders { get; set; }
    public List<CustomersViewModel>? customersViews { get; set; }

    public CustomersViewModel? customershistory { get; set; }

    public decimal totalAmount { get; set; }

    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
}
