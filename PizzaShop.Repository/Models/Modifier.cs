using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Modifier
{
    public int Modifierid { get; set; }

    public string? Modifiername { get; set; }

    public decimal? Modifierrate { get; set; }

    public decimal? Modifierquantity { get; set; }

    public decimal? Modifierunit { get; set; }

    public string? Modifierdescription { get; set; }

    public decimal? Taxpercentage { get; set; }

    public bool? Taxdefault { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Deletedat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public int? Createdbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual ICollection<Modfierandgroupsmapping> Modfierandgroupsmappings { get; set; } = new List<Modfierandgroupsmapping>();

    public virtual ICollection<OrderItemModifiersMapping> OrderItemModifiersMappings { get; set; } = new List<OrderItemModifiersMapping>();
}
