using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Section
{
    public int Sectionid { get; set; }

    public string Sectionname { get; set; } = null!;

    public string? Sectiondescription { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<WaitingList> WaitingLists { get; set; } = new List<WaitingList>();
}
