using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class OrderItemModifiersMapping
{
    public int Mappingid { get; set; }

    public int Modifierid { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Createdbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public int Orderitemmappingid { get; set; }

    public virtual Modifier Modifier { get; set; } = null!;

    public virtual OrderItemMapping Orderitemmapping { get; set; } = null!;
}
