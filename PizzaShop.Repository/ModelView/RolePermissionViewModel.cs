using System;

namespace PizzaShop.Repository.ModelView;

public class RolePermissionModelView
{
    public int RoleId { get; set;}
    public int PermissionId { get; set; }
    public string? RoleName { get; set; }
    public string? PermissionName { get; set; }
    public bool Canview { get; set; }
    public bool Canedit { get; set; }
    public bool Candelete { get; set; }
}
