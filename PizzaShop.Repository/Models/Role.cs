using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Role
{
    public int Roleid { get; set; }

    public string Rolename { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<PermissionsRole> PermissionsRoles { get; set; } = new List<PermissionsRole>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
