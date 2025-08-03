using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Modifiergroup
{
    public int Modifiergroupid { get; set; }

    public string? Modifiergroupname { get; set; }

    public string? Modifiergroupdescription { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Deletedat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Editedbyid { get; set; }

    public int? Createdbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual ICollection<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; } = new List<ItemModifiergroupMapping>();

    public virtual ICollection<Modfierandgroupsmapping> Modfierandgroupsmappings { get; set; } = new List<Modfierandgroupsmapping>();
}
