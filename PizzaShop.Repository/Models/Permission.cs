using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Permission
{
    public int Permissionid { get; set; }

    public string Permissionname { get; set; } = null!;

    public virtual ICollection<PermissionsRole> PermissionsRoles { get; set; } = new List<PermissionsRole>();
}
