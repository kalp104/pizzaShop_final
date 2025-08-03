using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

public class EventsController : Controller
{

    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private readonly ICustomerService _customerService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EventsController(
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
        await FetchData();
        return View();
    }
}
