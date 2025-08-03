using System;

namespace PizzaShop.Repository.ModelView;

public class PermissionUpdateViewModel
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
    public string? Action { get; set; }
    public bool Value { get; set; }
}
