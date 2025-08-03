using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Filters;

public class AuthorizePermissionUser : ActionFilterAttribute
{
    private readonly IUserService _userService;

    public AuthorizePermissionUser(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        string? role = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        if(role == "Chef")
        {
                string path = context.HttpContext.Request.Path;
                if(path.Contains("Users/UserDashboard"))
                {
                    context.Result = new RedirectToActionResult("Privacy", "Home", null);
                    return;
                }
        }
        
        await next();
    }
}
