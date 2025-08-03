using System.Drawing;
using System.Security.Claims;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionCustomer))]
public class CustomerController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CustomerController(
        IUserService userService,
        IOrderService orderService,
        ICustomerService customerService,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _userService = userService;
        _orderService = orderService;
        _customerService = customerService;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task FetchData()
    {
        string? role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;

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
        await FetchData();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FilterCustomers(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null, // e.g., "name", "date", "total"
        string? sortDirection = null // e.g., "asc", "desc"
    )
    {
        await FetchData();
        OrdersHelperModelView? customer = await _customerService.GetCustomers(
            searchTerm,
            pageNumber,
            pageSize,
            dateRange,
            fromDate,
            toDate,
            sortBy,
            sortDirection
        );
        return PartialView("_CustomerPartial", customer);
    }

    [HttpGet]
    public async Task<IActionResult> customerDetails(int customerId)
    {
        await FetchData();
        OrdersHelperModelView? result = await _customerService.getCustomerHistory(customerId);
        return PartialView("_CustomerHistory", result);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCustomers(
        string? searchTerm = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null
    )
    {
        await FetchData();
        string? userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        (byte[] fileContent, string fileName) = await _customerService.ExportCustomers(
            searchTerm,
            dateRange,
            fromDate,
            toDate,
            userRole
        );

        return File(
            fileContent,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
}
