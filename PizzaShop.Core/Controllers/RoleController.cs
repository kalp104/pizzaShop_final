using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionRoles))]
public class RoleController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    private readonly IRoleService _roleService;

    public RoleController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService,
        IRoleService roleService
    )
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
        _roleService = roleService;
    }

    private async Task FetchData()
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.role = role;
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
    }

    public async Task<IActionResult> Role()
    {
        await FetchData();
        List<Role>? result = await _roleService.GetRoles();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Permission(int id)
    {
        try{
            await FetchData();
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            List<RolePermissionModelView>? result = await _roleService.RoleBasePermission(id);
            int roleid = 0;
            switch (role)
            {
                case "AccountManager":
                    roleid = 1;
                    break;
                case "Chef":
                    roleid = 2;
                    break;
                case "Admin":
                    roleid = 3;
                    break;
                default :
                    throw new Exception("Error 500 : role is not defined!");
            }

            List<RolePermissionModelView>? permissions = await _roleService.RoleBasePermission(roleid);
            bool Save = true;
            if (permissions != null)
            {
                foreach (var i in permissions)
                {
                    if (i.PermissionId == 2 && i.Canedit == false)
                    {
                        Save = false;
                        break;
                    }
                }
            }

            switch (id)
            {
                case 1:
                    ViewBag.Rolename = "AccountManager";
                    break;
                case 2:
                    ViewBag.Rolename = "Chef";
                    break;
                case 3:
                    ViewBag.Rolename = "Admin";
                    break;
                default :
                    throw new Exception("Error 500 : Error while fetching permission!");
            }


            ViewBag.Save = Save;
            return View(result ?? new List<RolePermissionModelView>());
        }
        catch (Exception e)
        {
            TempData["ErrorPermission"] = "Error 500 : error while fetching permission data";
            return RedirectToAction("Role"); 
        }
        
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePermission(List<RolePermissionModelView> model)
    {
        try{
            await FetchData();

            if (ModelState.IsValid)
            {
                await _roleService.UpdatePermissions(model);
                TempData["SuccessPermission"] = "Permissions updated successfully";
                return RedirectToAction("Permission", new { id = model.FirstOrDefault()?.RoleId });
            }
            return View("Permission", model);
        }
        catch (Exception e)
        {
            string error = "error 500 : " + e.Message; 
            TempData["ErrorPermission"] = error;
            return View("Permission", model); 
        }
    }
}
