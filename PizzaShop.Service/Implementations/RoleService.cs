using System;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;

    public RoleService(
        IRoleRepository roleRepository
    )
    {   
        _roleRepository = roleRepository;
    }

    public async Task<List<Role>?> GetRoles()
    {
        return await _roleRepository.GetAllRoles();
    }

    public async Task<List<RolePermissionModelView>?> RoleBasePermission(int id)
    {
        List<RolePermissionModelView>? result = await _roleRepository.GetPermissionAsync(id);
        if(id == 2 || id == 1)
        {
            if (result != null)
            {
                result = result.Where(i => i.PermissionId != 2).ToList();
            }
        }
        return result;
    }

    public async Task UpdatePermissions(List<RolePermissionModelView> models)
    {
        foreach (var model in models)
        {
            PermissionsRole? permissionsRole = await _roleRepository.GetRoleAndPermissionAsync(
                model.RoleId,
                model.PermissionId
            );
            if (permissionsRole != null)
            {
                if(model.RoleId == 3 && model.PermissionId == 2)
                {
                    model.Candelete = true;
                    model.Canedit = true;
                    model.Canview = true;
                }
                
                permissionsRole.Canview = model.Canview;
                permissionsRole.Canedit = model.Canedit;
                permissionsRole.Candelete = model.Candelete;
                await _roleRepository.UpdatePermissionsRole(permissionsRole);
            }
        }
    }
}
