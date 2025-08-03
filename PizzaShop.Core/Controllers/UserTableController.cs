using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionUserTable))]
public class UserTableController : Controller
{
    private readonly IUserService _userService;
    private readonly IUserTableService _userTableService;

    public UserTableController(IUserService userService, IUserTableService userTableService)
    {
        _userTableService = userTableService;
        _userService = userService;
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

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Request.Cookies["auth_token"] != null)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return RedirectToAction("index", "Home");
            }
            
            await FetchData();
            return View();
        }
        return RedirectToAction("Index", "Home");
    }




    [HttpGet]
    public async Task<IActionResult> GetUsers(
        int page = 1,
        int pageSize = 5,
        string searchTerm = "",
        string? sortBy = null, //  name,role
        string? sortDirection = null //  "asc", "desc"
    )
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == null)
        {
            return RedirectToAction("Index", "Home");
        }

        //role and permission vise edit and delelete permission
        // 1 for users
        // 2 for roles and permission etc...
        RolePermissionModelView? roleFilter = await _userService.PermissionFilter(role, 1);
        List<UserTableViewModel>? result = await _userTableService.GetUsersDetails(page, pageSize, searchTerm, sortBy, sortDirection);
        int totalCount = await _userTableService.GetUsersCount(searchTerm);
        if (result == null)
            return View();
            
        return Json(
            new
            {
                data = result,
                totalUsers = totalCount,
                roleFilter?.Canedit,
                roleFilter?.Candelete,
            }
        );
    }



    public async Task<IActionResult> DeleteUserById(int id)
    {
        try{
            await FetchData();
            bool res = await _userTableService.DeleteUser(id,ViewBag.Userid);
            if(res){
                return Json(new { success = true, message = "User deleted successfully." });
            }
            else{
                return Json(new { success = false, message = "User not found." });
            }
            
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting user: " + ex.Message });
        }
        
    }





    [HttpGet]
    public async Task<IActionResult> EditUserById(int id)
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        await FetchData();
        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }
        UserViewModel user = await _userTableService.EditUserById(id);
        return View(user);
    }





    [HttpPost]
    public async Task<IActionResult> EditUserById(
        UserViewModel model,
        [FromForm] IFormFile imageFile
    )
    {
        try{
            string? email = User.FindFirst(ClaimTypes.Email)?.Value;
            await FetchData();
            if (email == null)
            {
                return RedirectToAction("index", "Home");
            }
            if (model.userId != 0)
            {
                bool res = await _userTableService.EditUserPostAsync(model, imageFile);
                if (res)
                {
                    TempData["success"] = "User updated successfully.";
                    return Json( new { success = true, message = "User updated successfully." });
                    
                }
                else
                {
                    return Json(new { success = false, message = "Username already exists!" });
                }
                
            }
            else
            {
                return Json(new { success = false, message = "User not found!" });
            }
        }
        catch (Exception ex)
        {   
           return Json(new { success = false, message = "Error updating user: " + ex.Message });
        }
        

       
    }

    [HttpGet]
    public async Task<IActionResult> GetStates(int countryId)
    {
        List<State>? states = await _userService.GetStates(countryId);
        return Json(states);
    }

    [HttpGet]
    public async Task<IActionResult> GetCities(int stateId)
    {
        List<City>? city = await _userService.GetCities(stateId);
        return Json(city);
    }

    public async Task<IActionResult> AddUser()
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        await FetchData();

        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }

        List<Country1>? countries = await _userService.GetCountries();
        ViewBag.Country = countries;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser(
        AddNewUserViewModel obj,
        [FromForm] IFormFile imageFile
    )
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        await FetchData();
        if (email == null)
        {
            return RedirectToAction("index", "Home");
        }

        if (ModelState.IsValid)
        {
            string? error = await _userTableService.AddUserService(obj, imageFile, ViewBag.Userid);
            if (error != "")
            {
                TempData["error"] = error;
                List<Country1>? c = await _userService.GetCountries();
                ViewBag.Country = c;
                return View(obj);
            }

            // login service
            string ReciverMail = obj.Email ?? "";
            string ReciverPassword = obj.Password ?? "";

            if(ReciverMail != null && ReciverPassword != null)
            {
                bool res = await _userTableService.EmailSendToNewUser(ReciverMail, ReciverPassword);
            }

            return RedirectToAction("Index");
        }
        List<Country1>? countries = await _userService.GetCountries();
        ViewBag.Country = countries;
        return View(obj);
    }
}
