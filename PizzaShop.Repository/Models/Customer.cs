using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Customer
{
    public int Customerid { get; set; }

    public string Customername { get; set; } = null!;

    public string Customeremail { get; set; } = null!;

    public decimal Customerphone { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public virtual ICollection<EventCustomerMapping> EventCustomerMappings { get; set; } = new List<EventCustomerMapping>();

    public virtual ICollection<OrdersCustomersMapping> OrdersCustomersMappings { get; set; } = new List<OrdersCustomersMapping>();
}
