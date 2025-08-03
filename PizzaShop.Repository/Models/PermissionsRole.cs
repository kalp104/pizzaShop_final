using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class PermissionsRole
{
    public int Permissionroleid { get; set; }

    public int Permissionid { get; set; }

    public int Roleid { get; set; }

    public bool? Canview { get; set; }

    public bool? Canedit { get; set; }

    public bool? Candelete { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Deletedbyid { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Permission Permission { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
