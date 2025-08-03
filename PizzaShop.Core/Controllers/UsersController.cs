using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionUser))]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;

    public UsersController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService
    )
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
    }

    public async Task FetchData()
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

    public IActionResult RoleWiseBack()
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "Admin" || role == "AccountManager")
        {
            TempData["success"] = "logged in";
            return RedirectToAction("UserDashboard", "Users");
        }else if(role == "Chef"){
            TempData["success"] = "logged in";
            return RedirectToAction("Index", "OrderApp");
        }
        return RedirectToAction("index", "Home");
    }

    [Authorize]
    public async Task<IActionResult> UserDashboard()
    {
        await FetchData();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile()
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
            ViewBag.Role = role;
        }
        await FetchData();
        UserViewModel? user =
            userBag != null ? await _userTableService.EditUserById(userBag.UserId) : null;

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UserProfile(UserViewModel model)
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
        await FetchData();
        string? result = await _userService.UpdateUserProfile(model);
        if (result == "")
        {
            UserViewModel? user =
                userBag != null ? await _userTableService.EditUserById(userBag.UserId) : null;
            TempData["updated"] = "user updated successfully";
            return View(user);
        }

        TempData["ERROR"] = result +" Profile Update Failed";
        return RedirectToAction("UserProfile", model);
    }

    public async Task<IActionResult> UpdateImage(int userId, IFormFile imageFile)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            TempData["ERROR"] = "Please select the a image.";
            return RedirectToAction("UserProfile");
        }
        await FetchData();
        string? result = await _userService.UpdateImage(userId, imageFile);

        if (result == "")
            return RedirectToAction("UserProfile");
        ViewBag.ImageUrl = result;
        TempData["updated"] = "image updated successfully.";
        return RedirectToAction("UserProfile");
    }

    [HttpGet]
    public async Task<IActionResult> ChangePassword()
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        await FetchData();
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model)
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
        await FetchData();
        if (!ModelState.IsValid)
        {
            return View(model); // Return view if input validation fails
        }
        string? result = await _userService.ChangePassword(model, email);
        switch (result)
        {
            case "0":
                TempData["changepassword"] = "Password updated successfully";
                return View();
            case "1":
                ModelState.AddModelError("", "Account not found.");
                return View(model);
            case "2":
                ModelState.AddModelError("CurrentPassword", "Incorrect current password.");
                return View(model);
            case "3":
                ModelState.AddModelError(
                    "Password",
                    "New password cannot be the same as the old one."
                );
                return View(model);
            case "4":
                ModelState.AddModelError("ConfirmPassword", "Password does not match.");
                return View(model);
        }
        TempData["changePasswordError"] = "Somthing went wrong";
        return View();
    }

    #region "User Dashboard"
    [HttpGet]
    public async Task<IActionResult> UserDashboardData(int? selector = null,DateTime? fromDate = null, DateTime? toDate = null)
    {
        try{
            await FetchData();
            DashBoardViewModel? result = await _userService.GetDashboardData(selector, fromDate, toDate);
            
            if(result == null)
            {
                return Json(new {success = false, message = "No data found"});
            }
            return Json(new {success = true, data = result});
        }
        catch (Exception ex)
        {
            return Json(new {success = false, message = ex.Message});
        }
        

    }
    #endregion
}
