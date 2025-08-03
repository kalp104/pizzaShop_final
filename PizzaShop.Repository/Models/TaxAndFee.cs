using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class TaxAndFee
{
    public int Taxid { get; set; }

    public string Taxname { get; set; } = null!;

    public int Taxtype { get; set; }

    public decimal Taxamount { get; set; }

    public bool? Isenabled { get; set; }

    public bool? Isdefault { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual ICollection<OrderTaxMapping> OrderTaxMappings { get; set; } = new List<OrderTaxMapping>();
}
