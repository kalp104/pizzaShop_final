using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Filters;

public class AuthorizePermissionUserTable : ActionFilterAttribute
{
    private readonly IUserService _userService;

    public AuthorizePermissionUserTable(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        string? role = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);

        if (rolefilter != null)
        {
            foreach (RolePermissionModelView i in rolefilter)
            {
                if (i.PermissionId == 1 && i.Canview == false)
                {
                    context.Result = new RedirectToActionResult("Privacy", "Home", null);
                    return;
                }
            }
        }
        await next();
    }
}
