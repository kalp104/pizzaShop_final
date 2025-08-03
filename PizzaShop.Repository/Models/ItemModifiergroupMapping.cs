using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class ItemModifiergroupMapping
{
    public int Itemmodifierid { get; set; }

    public int Itemid { get; set; }

    public int Modifiergroupid { get; set; }

    public int? Minvalue { get; set; }

    public int? Maxvalue { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual Item Item { get; set; } = null!;

    public virtual Modifiergroup Modifiergroup { get; set; } = null!;
}
