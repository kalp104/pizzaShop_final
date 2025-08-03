using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class WaitingList
{
    public int Waitingid { get; set; }

    public int Sectionid { get; set; }

    public string Customername { get; set; } = null!;

    public string Customeremail { get; set; } = null!;

    public decimal Customerphone { get; set; }

    public int Totalperson { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual Section Section { get; set; } = null!;
}
