using System;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly PizzaShop2Context _context;

    public RoleRepository(PizzaShop2Context context)
    {
        _context = context;       
    }


    public async Task<List<Role>?> GetAllRoles()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetRoleById(int roleid)
    {
        return await _context.Roles.Where(x => x.Roleid == roleid).FirstOrDefaultAsync();
    }
    
    public async Task<List<RolePermissionModelView>?> GetPermissionAsync(int id)
    {
            var result = await (
                from pr in _context.PermissionsRoles
                join r in _context.Roles on pr.Roleid equals r.Roleid
                join p in _context.Permissions on pr.Permissionid equals p.Permissionid
                where r.Roleid == id
                orderby pr.Permissionroleid
                select new RolePermissionModelView
                {
                    RoleId = id,
                    PermissionId = p.Permissionid,
                    RoleName = r.Rolename,
                    PermissionName = p.Permissionname,
                    Canview = (bool)pr.Canview,
                    Canedit = (bool)pr.Canedit,
                    Candelete = (bool)pr.Candelete,
                }
            ).ToListAsync();
            return result;
    }

    public async Task<List<RolePermissionModelView>?> GetRolePermissionModelViewAsync(int roleid)
    {
            List<RolePermissionModelView> result = await(
                from pr in _context.PermissionsRoles
                join r in _context.Roles on pr.Roleid equals r.Roleid
                join p in _context.Permissions on pr.Permissionid equals p.Permissionid
                where r.Roleid == roleid
                select new RolePermissionModelView
                {
                    RoleId = r.Roleid,
                    PermissionId = p.Permissionid,
                    Canview = (bool)pr.Canview,
                    Canedit = (bool)pr.Canedit,
                    Candelete = (bool)pr.Candelete,
                }
            ).ToListAsync();
            return result;
    }

    public async Task<PermissionsRole?> GetRoleAndPermissionAsync(int roleid, int permissionid)
    {
        return await _context.PermissionsRoles.FirstOrDefaultAsync(p => p.Roleid == roleid && p.Permissionid == permissionid);
    }

    public async Task UpdatePermissionsRole(PermissionsRole permissionsRole)
    {
        _context.Update(permissionsRole);
        await _context.SaveChangesAsync();
    }


    public async Task<RolePermissionModelView?> GetPermissionAsync(int roleid, int permission)
    {
            RolePermissionModelView? result = await (
                from pr in _context.PermissionsRoles
                join r in _context.Roles on pr.Roleid equals r.Roleid
                join p in _context.Permissions on pr.Permissionid equals p.Permissionid
                where r.Roleid == roleid && p.Permissionid == 1
                select new RolePermissionModelView
                {
                    RoleId = roleid,
                    PermissionId = p.Permissionid,
                    Canview = (bool)pr.Canview,
                    Canedit = (bool)pr.Canedit,
                    Candelete = (bool)pr.Candelete,
                }
            ).FirstOrDefaultAsync();
            return result;
    }
}
