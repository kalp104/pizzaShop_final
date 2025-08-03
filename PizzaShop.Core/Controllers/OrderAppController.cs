using System;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PizzaShop.Core.Filters;
using PizzaShop.Core.hubs;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionOrderApp))]
public class OrderAppController : Controller
{
    private readonly IUserService _userService;
    private readonly IMenuService _menuService;
    private readonly IOrderAppService _orderService;
    private readonly ISectionService _sectionService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHubContext<OrderAppHub> _hubContext;

    public OrderAppController(
        IUserService userService,
        IWebHostEnvironment webHostEnvironment,
        IMenuService menuService,
        ISectionService sectionService,
        IOrderAppService orderService,
        IHubContext<OrderAppHub> hubContext
    )
    {
        _userService = userService;
        _webHostEnvironment = webHostEnvironment;
        _menuService = menuService;
        _orderService = orderService;
        _sectionService = sectionService;
        _hubContext = hubContext;
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

    #region KOT
    public async Task<IActionResult> Index()
    {
        await FetchData();
        List<Category>? categories = await _menuService.GetAllCategories();
        OrderAppKOTViewModel result = new();
        result.categories = categories;
        return View(result);
    }

    public async Task<IActionResult> GetCardDetails(int? category = null, int? status = null, bool? IsModal = false)
    {
        OrderAppKOTViewModel result = await _orderService.GetCardDetails(category, status, IsModal);
        if (status != null)
        {
            result.StateForPartial = status;
        }
        return PartialView("_CardsAtKOTPartial", result);
    }

    [HttpGet]
    public async Task<IActionResult> GetModalDetails(int orderid, int? status = null, bool? IsModal = false)
    {
        OrderAppKOTViewModel result = await _orderService.GetCardDetails(null, status,IsModal);
        OrderKOTViewModel? res = new();
        if (result.orderKOT != null)
        {
            res = result.orderKOT.Where(o => o.orderId == orderid).FirstOrDefault();
        }

        return PartialView("_KOTModalPartial", res);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReadyQuantities(
        int orderId,
        List<UpdateReadyQuantityModel> updates,
        int Status 
    )
    {
        try
        {
            await _orderService.UpdateReadyQuantitiesAsync(orderId, updates, Status);
            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateReadyQuantities: {ex}");
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    #endregion





    #region Menu
    
    public async Task<IActionResult> Menu(int? tableId = null)
    {
        await FetchData();
        OrderAppMenuViewModel? menu = new ();
        if (tableId.HasValue)
        {
            menu.Tableid = tableId.Value;
        }
        // Always fetch categories to ensure they’re up-to-date
        List<Category>? categories = await _menuService.GetAllCategories();
        menu.categories = categories ?? new List<Category>();

        return View(menu);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderPageDetails(int tableId)
    {
        await FetchData();
        OrderAppMenuViewModel? result = await _orderService.GetOrderPageDetailByTableId(tableId);
        if (result != null)
        {
            return PartialView("_OrderPagePartial", result);
        }
        else
        {
            return Json(new { success = false, message = "No data found" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderDataIfExists(int OrderId)
    {
        await FetchData();
        OrderDataViewModel? result = await _orderService.GetOrderItemDetails(OrderId);
        if (result != null)
        {
            return Json(new { success = true, data = result });
        }
        else
        {
            return Json(new { success = false, message = "No data found" });
        }
    }
    

    public async Task<IActionResult> GetItemsByCategoryId(
        int? category = null,
        string? searchTerm = null
    )
    {
        await FetchData();
        List<Category>? categories = await _menuService.GetAllCategories();
        List<Item>? items = await _menuService.GetItemsByCategoryId(category, searchTerm);
        OrderAppMenuViewModel result = new();
        result.categories = categories;
        result.items = items;
        return PartialView("_CategoryCardsPartial", result);
    }

    [HttpGet]
    public async Task<IActionResult> AssignTableToMenu(int tableId)
    {
        OrderAppMenuViewModel ans = await _orderService.GetOrderPageDetailByTableId(tableId);
        if (ans != null)
        {
            // Redirect to Menu with tableId instead of using TempData
            return RedirectToAction("Menu", new { tableId = tableId });
        }
        else
        {
            TempData["Error"] = "Failed to assign table";
            return RedirectToAction("Tables");
        }
    }

    
    public async Task<IActionResult> GetModifiers(int itemid)
    {
        ItemsModifierViewModel res = new ();
        List<ModifierGroupDataHelperViewModel>? model = await _menuService.GetModifierGroupsByItemId(itemid);
        res.models = model;
        return PartialView("_ModifiersModalPartial",res);

    }



    [HttpPost]
    public async Task<IActionResult> SaveOrder([FromBody] OrderDataViewModel orderData)
    {
        try{
            if (orderData == null || orderData.Items == null || !orderData.Items.Any())
            {
                return Json(new { success=false, message = "Can not Save empty order! select the Items"});
            }

            bool ans = await _orderService.AddOrderDetails(orderData);

            if(ans == true)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOrderApp");
                return Json(new { success=true, message = "Order saved successfully"}); 
            }
            else{
                return Json(new { success=false, message = "Error while saving order"}); 
            }
        }
        catch(Exception e)
        {
            return Json(new { success=false, message = e.Message}); 
        }

    }

    [HttpGet]
    public async Task<IActionResult> CompleteOrder(int orderid,string feedback)
    {
        FeedbackViewModel? feedbackViewModel = JsonSerializer.Deserialize<FeedbackViewModel>(feedback);
        int status = 0;
        if(feedbackViewModel!=null)
        {
            await FetchData();
            status = await _orderService.CompleteOrder(orderid, ViewBag.Userid,feedbackViewModel);
            if(status == 1)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveOrderApp");
            }
        }
        
        return Json(new{
            status = status
        });
    }

    public async Task<IActionResult> CheckStateOfItems(int orderid)
    {
        try{
            bool res = await _orderService.CheckStateOfItems(orderid);
            if(res)
            {
                return Json(new {
                    success = true,
                    message = ""
                });
            }else{
                return Json(new {
                    success = false,
                    message = "Some Items are not ready yet! can not complete the order"
                });
            }
        }catch(Exception e)
        {
            return Json(new {
                success = false,
                message = e.Message
            });
        }
    }


    [HttpGet]
    public async Task<IActionResult> CancelOrder(int OrderId)
    {
        await FetchData();
        bool status = await _orderService.CancelOrder(OrderId,ViewBag.Userid);
        
        if(status == true)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveOrderApp");
        }
        
        return Json(new {
            status = status
        });
        
    }

    public async Task<IActionResult> EditCustomer(OrderAppMenuViewModel model)
    {
        try{
            await FetchData();
            int userid = ViewBag.Userid;
            bool res = false;
            if(model.customer!=null){
                res = await _orderService.EditCustomerDetails(model.customer, userid);
            }
            if(res)
            {
                return Json(new {
                    success = true,
                    message=""
                });
            }
            return Json(new {
                    success = false,
                    message = "Table's capacity reached!" 
            });
            
        }catch(Exception e)
        {
            return Json(new {
                success = false,
                message = e.Message
            });
        }
        
    }


    public async Task<IActionResult> AddFavourite(int itemid)
    {
        bool res = await _orderService.AddFavourite(itemid);
        if(res)
        {
            return Json(new{
                status = true
            });
        }
        return Json(new {
            status = false
        });
    }



    #endregion





    #region Tables
    public async Task<IActionResult> Tables()
    {
        await FetchData();
        OrderAppTableViewModel? result = await _sectionService.GetAllSections();
        result.waitingList = await _orderService.GetAllWaitingList();

        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingTokenBySectionId(int? sectionId = null)
    {
        try{
            if(sectionId!=null)
            {
                List<WaitingListViewModel>? list = await _orderService.GetAllWaitingListBySectionId(sectionId);
                if(list!=null)
                {
                    OrderAppTableViewModel? res = new();
                    res.waitingList = list;
                    return PartialView("_ofcanvasWaitingListPartial",res);
                }
            }
           
                return Json(new {
                    success = false,
                    message = "Error Occured while fetching tokens"
                }); 
            
        }
        catch (Exception e)
        {
            return Json(new {
                success = false,
                message = e.Message
            });
        }
    }

    public async Task<IActionResult> AddWaitingToken(OrderAppTableViewModel model)
    {
        if (ModelState.IsValid)
        {
            await FetchData();
            int userid = ViewBag.Userid;
            bool ans = await _sectionService.AddWaitingToken(model, userid);
            TempData["Success"] = "Token Added Successfully";
            return RedirectToAction("Tables");
        }
        else
        {
            OrderAppTableViewModel? result = await _sectionService.GetAllSections();
            return View("Tables", result);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCustomer(OrderAppTableViewModel model)
    {
        try{
            await FetchData();
            int userid = ViewBag.Userid;
            if (model.tableIds != null)
            {
                List<int> tableIdsList = new List<int>();
                string[] tableIds = model.tableIds.Split(',');
                foreach (string tableId in tableIds)
                {
                    int id = Convert.ToInt32(tableId);
                    tableIdsList.Add(id);
                }
                model.Tableids = tableIdsList;
            }
            OrderAppMenuViewModel? ans = await _orderService.AddCustomer(model, userid);

            if(ans!=null)
            {
                TempData["Success"] = "Customer Added Successfully";
                int? tableId = 0;
                if(model.Tableids != null && model.Tableids.Count > 0)
                {
                    tableId = model.Tableids?.FirstOrDefault();
                }
                else if(model.Tableid != 0)
                {
                    tableId = model.Tableid;
                }
                
                return Json(new {
                    success = true,
                    tableId = tableId,
                    message = "Table is assign to customer!"
                });
            } 
            else
            {
                return Json(new {
                    success = false,
                    message = "Tables Capacity reached!"
                });
            }
        }
        catch(Exception e)
        {
            System.Console.WriteLine(e.Message);
            return Json(new {
                    success = false,
                    message = e.Message
                });
        }
    }

    #endregion





    #region waiting list
    public async Task<IActionResult> WaitingList(int? sectionId = null)
    {
        await FetchData();
        OrderAppWaitingTokenViewModel list = await _orderService.GetWaitingTokens(sectionId);
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> GetWaitingLists(int? sectionId = null)
    {
        await FetchData();
        OrderAppWaitingTokenViewModel list = await _orderService.GetWaitingTokens(sectionId);
        return PartialView("_WaitingListPartial", list);
    }

    public async Task<IActionResult> AddWaitingList(OrderAppTableViewModel model)
    {
        if (ModelState.IsValid)
        {
            await FetchData();
            int userid = ViewBag.Userid;
            bool ans = await _sectionService.AddWaitingToken(model, userid);
            TempData["Success"] = "Token Added Successfully";
            return RedirectToAction("WaitingList");
        }
        else
        {
            OrderAppTableViewModel? result = await _sectionService.GetAllSections();
            return View("WaitingList", result);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerDetails(string email)
    {
        await FetchData();
        OrderAppWaitingTokenViewModel? result = await _orderService.GetCustomerDetails(email);
        if (result.found)
        {
            return PartialView("_CustomerDetailsPartial", result);
        }
        else
        {
            return new JsonResult(new { success = false });
        }
    }

    public async Task<IActionResult> DeleteWaitingToken(int Waitingid)
    {
        await FetchData();
        bool ans = await _orderService.DeleteWaitingToken(Waitingid, ViewBag.Userid);
        if (ans)
        {
            TempData["Success"] = "Token Deleted Successfully";
        }
        else
        {
            TempData["Error"] = "Token Not Deleted";
        }
        return RedirectToAction("WaitingList");
    }

    public async Task<IActionResult> EditWaitingList(OrderAppTableViewModel model)
    {
        if (ModelState.IsValid)
        {
            await FetchData();
            int userid = ViewBag.Userid;
            bool ans = await _orderService.EditWaitingToken(model, userid);
            if (ans)
            {
                TempData["Success"] = "Token Updated Successfully";
            }
            else
            {
                TempData["Error"] = "Token Not Updated";
            }
            return RedirectToAction("WaitingList");
        }
        else
        {
            OrderAppTableViewModel? result = await _sectionService.GetAllSections();
            return View("WaitingList", result);
        }
    }

    public async Task<IActionResult> GetTables(int? sectionId = null, int? capacity = null)
    {
        await FetchData();
        List<Table>? tables = await _orderService.GetTablesBySectionId(sectionId, capacity);
        if(tables!=null && tables.Count > 0)
        {
            return new JsonResult(new { success = true, data = tables });
        }
        else return new JsonResult(new { success = false });
    }

    #endregion
}
