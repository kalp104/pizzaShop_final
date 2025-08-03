using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>?> GetAllRoles();
    Task<Role?> GetRoleById(int roleid); 
    Task<List<RolePermissionModelView>?> GetPermissionAsync(int id);
    Task<RolePermissionModelView?> GetPermissionAsync(int roleid, int permission);
    Task<List<RolePermissionModelView>?> GetRolePermissionModelViewAsync(int roleid);
    Task<PermissionsRole?> GetRoleAndPermissionAsync(int roleid, int permissionid);
    Task UpdatePermissionsRole(PermissionsRole permissionsRole);
}
