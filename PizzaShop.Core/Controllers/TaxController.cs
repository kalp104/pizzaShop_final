using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionTax))]
public class TaxController : Controller
{
    private readonly IUserService _userService;
    private readonly ITaxService _taxService;

    public TaxController(IUserService userService, ITaxService taxService)
    {
        _userService = userService;
        _taxService = taxService;
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

    public async Task<IActionResult> Index()
    {
        TaxHelperViewModel tax = await _taxService.GetTaxesAsync(null, null);
        ViewBag.SeclectedTaxId = tax.Taxid;
        await FetchData();
        return View();
    }

    public async Task<IActionResult> FatchTaxes(int? taxId = null, string? searchTerm = null)
    {
        TaxHelperViewModel tax = await _taxService.GetTaxesAsync(taxId, searchTerm);
        ViewBag.SeclectedTaxId = tax.Taxid;
        await FetchData();
        return PartialView("_TaxPartial", tax);
    }

    [HttpPost]
    public async Task<IActionResult> AddTax(TaxHelperViewModel model)
    {
        await FetchData();
        int userid = ViewBag.Userid;
        bool res = await _taxService.AddTaxAsync(model, userid);
        var response = new
        {
            success = res,
            message = res ? "Tax added successfully" : "Tax Already Exists"
        };
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTax(int taxId)
    {
        await FetchData();
        bool res = await _taxService.DeleteTaxAsync(taxId, ViewBag.Userid);
        var response = new
        {
            success = res,
            message = res ? "Tax deleted successfully" : "Error deleting tax"
        };
        return Json(response);
    }

    [HttpPost]
    public async Task<IActionResult> EditTax(TaxHelperViewModel model)
    {
        await FetchData();
        bool res = await _taxService.EditTaxAsync(model, ViewBag.Userid);
        var response = new
        {
            success = res,
            message = res ? "Tax updated successfully" : "Tax Name Already Exists"
        };
        return Json(response);
    }
}