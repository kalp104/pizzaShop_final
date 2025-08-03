using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IRoleService
{
    public Task<List<Role>?> GetRoles();
    public Task<List<RolePermissionModelView>?> RoleBasePermission(int id);
    public Task UpdatePermissions(List<RolePermissionModelView> models);
}
