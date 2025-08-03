using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class OrdersTablesMapping
{
    public int Orderstableid { get; set; }

    public int Orderid { get; set; }

    public int Tableid { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public int? Status { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Table Table { get; set; } = null!;
}
