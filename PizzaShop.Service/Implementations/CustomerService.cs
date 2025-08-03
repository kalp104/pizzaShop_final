using Microsoft.AspNetCore.Hosting;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace PizzaShop.Service.Implementations;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IOrderRepository _orderRepository;


    // private readonly IGenericRepository<Customer> _customer;
    // private readonly IGenericRepository<Order> _order;
    // private readonly IGenericRepository<OrdersCustomersMapping> _orderCustomerMapping;
    // private readonly IGenericRepository<OrderItemMapping> _orderItemMapping;

    public CustomerService(
        ICustomerRepository customerRepository,
        IWebHostEnvironment webHostEnvironment,
        IOrderRepository orderRepository        
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrdersHelperModelView> GetCustomers(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null,
        string? sortDirection = null
    )
    {
        List<Customer>? customer = await _customerRepository.GetAllCategoryAsync();

        if (customer != null)
        {

            // Search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customer = customer
                    .Where(s => s.Customername.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();
            }

            // Date range filter
            if (
                !string.IsNullOrEmpty(dateRange)
                && int.TryParse(dateRange, out int dateRangeValue)
                && Enum.IsDefined(typeof(DateRangeCustom), dateRangeValue)
            )
            {
                DateTime currentDate = DateTime.Today;
                switch ((DateRangeCustom)dateRangeValue)
                {
                    case DateRangeCustom.AllTime:
                        // No filtering needed
                        break;
                    case DateRangeCustom.Last7days:
                        customer = customer
                            .Where(o => o.Createdat >= currentDate.AddDays(-7))
                            .ToList();
                        break;
                    case DateRangeCustom.Last30days:
                        customer = customer
                            .Where(o => o.Createdat >= currentDate.AddDays(-30))
                            .ToList();
                        break;
                    case DateRangeCustom.CurrentMonth:
                        DateTime firstDayOfMonth = new DateTime(
                            currentDate.Year,
                            currentDate.Month,
                            1
                        );
                        customer = customer.Where(o => o.Createdat >= firstDayOfMonth).ToList();
                        break;
                }
            }

            // Custom date filter
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var from))
            {
                customer = customer.Where(o => o.Createdat >= from.Date).ToList();
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var to))
            {
                // Extend toDate to include the full day
                var toEndOfDay = to.Date.AddDays(1).AddTicks(-1); // End of the day (23:59:59.999)
                customer = customer.Where(o => o.Createdat <= toEndOfDay).ToList();
            }
        }

        List<CustomersViewModel>? result = new List<CustomersViewModel>();
        if (customer != null)
        {

            List<OrdersCustomersMapping>? mappings = await _customerRepository.GetAllOrdersCustomersMapping();
            foreach (Customer c in customer)
            {
                List<OrdersCustomersMapping> order = mappings
                    .Where(u => u.Customerid == c.Customerid)
                    .ToList();
                CustomersViewModel? mapping = new CustomersViewModel()
                {
                    totalOrders = order.Count,
                    Customeremail = c.Customeremail,
                    Customername = c.Customername,
                    Customerid = c.Customerid,
                    Customerphone = c.Customerphone,
                    Createdat = c.Createdat,
                    Createdbyid = c.Createdbyid,
                    Editedat = c.Editedat,
                    Editedbyid = c.Editedbyid,
                    Deletedat = c.Deletedat,
                    Deletedbyid = c.Deletedbyid,
                    Isdeleted = c.Isdeleted,
                };
                result.Add(mapping);
            }

            // Sorting logic with sortBy and sortDirection
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        result =
                            sortDirection?.ToLower() == "asc"
                                ? result.OrderBy(o => o.Customername).ToList()
                                : result.OrderByDescending(o => o.Customername).ToList();
                        break;
                    case "date":
                        result =
                            sortDirection?.ToLower() == "asc"
                                ? result.OrderBy(o => o.Createdat).ToList()
                                : result.OrderByDescending(o => o.Createdat).ToList();
                        break;
                    case "total":
                        result =
                            sortDirection?.ToLower() == "asc"
                                ? result.OrderBy(o => o.totalOrders).ToList()
                                : result.OrderByDescending(o => o.totalOrders).ToList();
                        break;
                    default:
                        // Default sorting (e.g., by date descending)
                        result = result.OrderByDescending(o => o.Createdat).ToList();
                        break;
                }
            }
            else
            {
                // Default sorting if no sortBy is specified
                result = result.OrderByDescending(o => o.Createdat).ToList();
            }
        }

        int totalItems = result?.Count ?? 0;
        result = result?.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new OrdersHelperModelView
        {
            customersViews = result,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
    }

    public async Task<OrdersHelperModelView> getCustomerHistory(int customerId)
    {
       
        Customer? c = await _customerRepository.GetCutomerById(customerId);
        
        if (c != null)
        {
            List<OrdersCustomersMapping>? order = await _customerRepository.GetOrdersCustomersMappingByCustomerId(c.Customerid);

            List<int> orderIds = order.Select(u => u.Orderid).ToList();

            List<Order> requireOrders = new List<Order>();

            foreach (int id in orderIds)
            {
                
                Order? o = await _orderRepository.GetOrderById(id);
                if (o != null)
                    requireOrders.Add(o);
            }

            List<OrderItemMapping>? orderItemMappings = await _orderRepository.GetAllOrderItemMappings();

            decimal avgbill = 0;
            decimal MaxOrders = int.MinValue;
            foreach (Order a in requireOrders)
            {
                avgbill += a.Totalamount;
                MaxOrders = Math.Max(a.Totalamount, MaxOrders); 
            }
            
            avgbill /= requireOrders.Count;

            CustomersViewModel? mapping = new CustomersViewModel()
            {
                totalOrders = requireOrders.Count,
                AvgBill = avgbill,
                MaxOrders = Math.Round(MaxOrders, 2),
                Customeremail = c.Customeremail,
                Customername = c.Customername,
                Customerid = c.Customerid,
                Customerphone = c.Customerphone,
                Createdat = c.Createdat,
                Createdbyid = c.Createdbyid,
                Editedat = c.Editedat,
                Editedbyid = c.Editedbyid,
                Deletedat = c.Deletedat,
                Deletedbyid = c.Deletedbyid,
                Isdeleted = c.Isdeleted,
            };

            List<OrderDetialsViewModel>? list = new();

            foreach (var a in requireOrders)
            {
                int TotalOrderedITems = 0;
                // geting total orders
                
                 TotalOrderedITems += orderItemMappings.Where(i => i.Orderid == a.Orderid).Count();
                
                
                OrderDetialsViewModel d = new()
                {
                    OrderDate = a.Createdat ?? null,
                    OrderType = a.Ordertype ?? 0,
                    Paymentmode = a.Paymentmode,
                    TotalOrderedITems = TotalOrderedITems,
                    amount = Math.Round(a.Totalamount,2),
                };
                list.Add(d);
            }
            mapping.OrderDetails = list;

            return new OrdersHelperModelView { customershistory = mapping };
        }

        return null;
    }

    public async Task<(byte[] FileContent, string FileName)> ExportCustomers(
    string? searchTerm = null,
    string? dateRange = null,
    string? fromDate = null,
    string? toDate = null,
    string? userRole = null
    )
    {
        OrdersHelperModelView? customer = await GetCustomers(
            searchTerm,
            1,
            int.MaxValue,
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
                logoImage.SetPosition(2, 0, 5, 0); // Row 2, Column 6
                logoImage.SetSize(90, 70); // Width: 90px, Height: 70px
            }
            else
            {
                worksheet.Cells[1, 1].Value = "Logo not found";
            }

            // Define column widths
            worksheet.Column(1).Width = 20; // #id
            worksheet.Column(2).Width = 35; // Name
            worksheet.Column(3).Width = 35; // Email
            worksheet.Column(4).Width = 25; // Date
            worksheet.Column(5).Width = 20; // Mobile no.
            worksheet.Column(6).Width = 15; // Total Order

            // Status Header
            worksheet.Cells[2, 1].Value = "Account";
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
            worksheet.Cells[2, 2].Value = userRole ?? string.Empty;
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
            if (!string.IsNullOrEmpty(dateRange) && int.TryParse(dateRange, out int drangeValue))
            {
                dateRangeValue = ((DateRangeCustom)drangeValue).ToString();
                if (dateRangeValue == "CustomDate")
                {
                    dateRangeValue = $"From {fromDate} to {toDate}";
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
            worksheet.Cells[6, 5].Value = customer.customersViews?.Count ?? 0;
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
            worksheet.Cells[10, 1].Value = "#ID";
            worksheet.Cells[10, 2].Value = "Name";
            worksheet.Cells[10, 3].Value = "Email";
            worksheet.Cells[10, 4].Value = "Date";
            worksheet.Cells[10, 5].Value = "Mobile No.";
            worksheet.Cells[10, 6].Value = "Total Orders";

            // Style headers
            using (var range = worksheet.Cells[10, 1, 10, 6])
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
            if (customer?.CurrentPage != null)
            {
                foreach (var order in customer?.customersViews)
                {
                    worksheet.Cells[row, 1].Value = order.Customerid;
                    worksheet.Cells[row, 2].Value = order.Customername ?? "customer";
                    worksheet.Cells[row, 3].Value = order.Customeremail ?? "N/A";
                    worksheet.Cells[row, 4].Value = order.Createdat?.ToString(
                        "yyyy-MM-dd HH:mm:ss"
                    );
                    worksheet.Cells[row, 5].Value = order.Customerphone;
                    worksheet.Cells[row, 6].Value = order.totalOrders;

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
                    row++;
                }
            }

            byte[] fileContent = package.GetAsByteArray();
            return (fileContent, "CustomersExport.xlsx");
        }
    }
}
