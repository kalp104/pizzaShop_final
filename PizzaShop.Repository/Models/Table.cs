using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Table
{
    public int Tableid { get; set; }

    public int Sectionid { get; set; }

    public string? Tablename { get; set; }

    public int? Capacity { get; set; }

    public int? Status { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual ICollection<OrdersTablesMapping> OrdersTablesMappings { get; set; } = new List<OrdersTablesMapping>();

    public virtual Section Section { get; set; } = null!;
}
