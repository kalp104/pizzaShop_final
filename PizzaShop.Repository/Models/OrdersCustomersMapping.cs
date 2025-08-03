using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class OrdersCustomersMapping
{
    public int OrderCustomerMappingId { get; set; }

    public int Customerid { get; set; }

    public int Orderid { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
