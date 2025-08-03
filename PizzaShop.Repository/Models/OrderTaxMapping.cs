using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class OrderTaxMapping
{
    public int Ordertaxmappingid { get; set; }

    public int Taxid { get; set; }

    public int Orderid { get; set; }

    public decimal Totalamount { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual TaxAndFee Tax { get; set; } = null!;
}
