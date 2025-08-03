using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Modfierandgroupsmapping
{
    public int Modfierandgroupsmappingid { get; set; }

    public int Modifierid { get; set; }

    public int Modifiergroupid { get; set; }

    public bool? Isdelete { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;
}
