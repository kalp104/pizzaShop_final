using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.ModelView;

public class PagedResultViewModel
{
    public List<UserTableViewModel>? Data { get; set; }
    public int TotalCount { get; set; }
}
