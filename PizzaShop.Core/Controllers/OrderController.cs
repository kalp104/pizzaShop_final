using System;
using System.Drawing;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Core.Controllers;

[Authorize(Roles = "Admin,AccountManager,Chef")]
[ServiceFilter(typeof(AuthorizePermissionOrders))]
public class OrderController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public OrderController(
        IUserService userService,
        IOrderService orderService,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _userService = userService;
        _orderService = orderService;
        _webHostEnvironment = webHostEnvironment;
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
        await FetchData();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FilterOrders(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        int? status = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null, //  "order", "date", "name", "amount"
        string? sortDirection = null //  "asc", "desc"
    )
    {
        await FetchData();
        OrdersHelperModelView? order = await _orderService.GetOrders(
            searchTerm,
            pageNumber,
            pageSize,
            status,
            dateRange,
            fromDate,
            toDate,
            sortBy,
            sortDirection
        );
        return PartialView("_OrderPartial", order);
    }

    [HttpGet]
    public async Task<IActionResult> OrderDetail(int orderId = 0)
    {
        await FetchData();
        OrderDetailsHelperViewModel? order = await _orderService.getOrderDetails(orderId);

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> GenerateInvoice(int? orderId)
    {
        if (!orderId.HasValue)
        {
            return BadRequest("Order ID is required.");
        }

        try
        {
            byte[] pdfBytes = await _orderService.GenerateInvoicePdf(orderId.Value);
            return File(pdfBytes, "application/pdf", $"Invoice_{orderId.Value}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error generating invoice: {ex.Message}");
        }
    }

    #region Export orders
    [HttpGet]
    public async Task<IActionResult> ExportOrders(
        string? searchTerm = null,
        int? status = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null
    )
    {
        await FetchData();
        OrdersHelperModelView orders = await _orderService.GetOrders(
            searchTerm,
            1,
            int.MaxValue,
            status,
            dateRange,
            fromDate,
            toDate,
            null,
            null
        );

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Orders");

            // Add logo image
            string logoPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                "images",
                "pizzashop_logo.png"
            );
            if (System.IO.File.Exists(logoPath))
            {
                var logoImage = worksheet.Drawings.AddPicture("Logo", new FileInfo(logoPath));
                logoImage.SetPosition(2, 0, 6, 0); // Row 2, Column 6
                logoImage.SetSize(90, 70); // Width: 90px, Height: 70px
            }
            else
            {
                worksheet.Cells[1, 1].Value = "Logo not found";
            }

            // Define column widths
            worksheet.Column(1).Width = 20; // #Order
            worksheet.Column(2).Width = 25; // Date
            worksheet.Column(3).Width = 20; // Customer
            worksheet.Column(4).Width = 25; // Status
            worksheet.Column(5).Width = 20; // Payment Mode
            worksheet.Column(6).Width = 15; // Rating
            worksheet.Column(7).Width = 20; // Total Amount

            // Status Header
            worksheet.Cells[2, 1].Value = "Status";
            worksheet.Cells[2, 1].Style.Font.Size = 13;
            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1, 3, 1].Merge = true;
            worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 120, 215));
            worksheet.Cells[2, 1].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Status Value
            worksheet.Cells[2, 2].Value = status.HasValue
                ? ((OrderStatus)status).ToString()
                : "All";
            worksheet.Cells[2, 2].Style.Font.Size = 12;
            worksheet.Cells[2, 2].Style.Font.Bold = true;
            worksheet.Cells[2, 2, 3, 2].Merge = true;
            worksheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
            worksheet.Cells[2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Searched Term Header
            worksheet.Cells[2, 4].Value = "Searched Term";
            worksheet.Cells[2, 4].Style.Font.Size = 13;
            worksheet.Cells[2, 4].Style.Font.Bold = true;
            worksheet.Cells[2, 4, 3, 4].Merge = true;
            worksheet.Cells[2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 120, 215));
            worksheet.Cells[2, 4].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Searched Term Value
            worksheet.Cells[2, 5].Value = string.IsNullOrEmpty(searchTerm) ? "None" : searchTerm;
            worksheet.Cells[2, 5].Style.Font.Size = 12;
            worksheet.Cells[2, 5].Style.Font.Bold = true;
            worksheet.Cells[2, 5, 3, 5].Merge = true;
            worksheet.Cells[2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[2, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[2, 5].Style.Fill.BackgroundColor.SetColor(Color.White);
            worksheet.Cells[2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Date Range Header
            worksheet.Cells[6, 1].Value = "Date Range";
            worksheet.Cells[6, 1].Style.Font.Size = 13;
            worksheet.Cells[6, 1].Style.Font.Bold = true;
            worksheet.Cells[6, 1, 7, 1].Merge = true;
            worksheet.Cells[6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[6, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[6, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 120, 215));
            worksheet.Cells[6, 1].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[6, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            string dateRangeValue = "All"; // Default value
            if (fromDate != null && toDate != null)
            {
                dateRangeValue = $"From {fromDate} to {toDate}";   
            }
            else{
                if(!string.IsNullOrEmpty(dateRange) && int.TryParse(dateRange, out int drangeValue)){
                    dateRangeValue = ((DateRangeCustom)drangeValue).ToString();
                }
            }
            worksheet.Cells[6, 2].Value = dateRangeValue;
            worksheet.Cells[6, 2].Style.Font.Size = 12;
            worksheet.Cells[6, 2].Style.Font.Bold = true;
            worksheet.Cells[6, 2, 7, 2].Merge = true;
            worksheet.Cells[6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[6, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[6, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, 2].Style.Fill.BackgroundColor.SetColor(Color.White);
            worksheet.Cells[6, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Total Records Header
            worksheet.Cells[6, 4].Value = "Total Records";
            worksheet.Cells[6, 4].Style.Font.Size = 13;
            worksheet.Cells[6, 4].Style.Font.Bold = true;
            worksheet.Cells[6, 4, 7, 4].Merge = true;
            worksheet.Cells[6, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[6, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[6, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 120, 215));
            worksheet.Cells[6, 4].Style.Font.Color.SetColor(Color.White);
            worksheet.Cells[6, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Total Records Value
            worksheet.Cells[6, 5].Value = orders.orders?.Count ?? 0;
            worksheet.Cells[6, 5].Style.Font.Size = 12;
            worksheet.Cells[6, 5].Style.Font.Bold = true;
            worksheet.Cells[6, 5, 7, 5].Merge = true;
            worksheet.Cells[6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[6, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Cells[6, 5].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[6, 5].Style.Fill.BackgroundColor.SetColor(Color.White);
            worksheet.Cells[6, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[6, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            // Headers
            worksheet.Cells[10, 1].Value = "#Order";
            worksheet.Cells[10, 2].Value = "Date";
            worksheet.Cells[10, 3].Value = "Customer";
            worksheet.Cells[10, 4].Value = "Status";
            worksheet.Cells[10, 5].Value = "Payment Mode";
            worksheet.Cells[10, 6].Value = "Rating";
            worksheet.Cells[10, 7].Value = "Total Amount";

            // Style headers
            using (var range = worksheet.Cells[10, 1, 10, 7])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 120, 215));
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            }

            // Data
            int row = 11;
            if (orders.orders != null)
            {
                foreach (var order in orders.orders)
                {
                    worksheet.Cells[row, 1].Value = order.Orderid;
                    worksheet.Cells[row, 2].Value = order.Createdat?.ToString(
                        "yyyy-MM-dd HH:mm:ss"
                    );
                    worksheet.Cells[row, 3].Value = order.Customername ?? "customer";
                    worksheet.Cells[row, 4].Value = ((OrderStatus)order.Status);
                    worksheet.Cells[row, 5].Value = ((PaymentMethod)order.Paymentmode);
                    worksheet.Cells[row, 6].Value = order.Ratings;
                    worksheet.Cells[row, 7].Value = order.Totalamount;

                    worksheet.Cells[row, 1].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 2].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 3].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 6].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 7].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Center;
                    row++;
                }
            }

            // Auto-fit columns (optional, remove if fixed widths are preferred)
            // worksheet.Cells.AutoFitColumns();

            // Return Excel file
            var stream = new MemoryStream(package.GetAsByteArray());
            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "OrdersExport.xlsx"
            );
        }

    #endregion
    }
}
