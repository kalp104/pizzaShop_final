using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Service.Implementations;

public class OrderService : IOrderService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ITaxRepository _taxRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IModifierRepository _modifierRepository;

    public OrderService(
        IWebHostEnvironment webHostEnvironment,
        ITaxRepository taxRepository,
        ISectionRepository sectionRepository,
        ITableRepository tableRepository,
        IOrderRepository orderRepository,
        IItemRepository itemRepository,
        IModifierRepository modifierRepository
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _taxRepository = taxRepository;
        _sectionRepository = sectionRepository;
        _tableRepository = tableRepository;
        _orderRepository = orderRepository;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
    }

    public async Task<OrdersHelperModelView> GetOrders(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        int? status = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null,
        string? sortDirection = null
    )
    {
        List<OrderCutstomerViewModel>? orders = await _orderRepository.GetAllCustomerOrderMappingAsync();
        if (orders != null)
        {
            // Search term filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders
                    .Where(o =>
                        (o?.Orderid != null && o.Orderid.ToString().Contains(searchTerm))
                        || (
                            o?.Customername != null
                            && o.Customername.ToLower().Contains(searchTerm.ToLower())
                        )
                    )
                    .ToList();
            }

            // Status filter
            if (status.HasValue)
            {
                orders = orders.Where(o => o.Status == status.Value).ToList();
            }

            // Date range filter
            if (
                !string.IsNullOrEmpty(dateRange)
                && fromDate == null
                && toDate == null
                && int.TryParse(dateRange, out int dateRangeValue)
                && Enum.IsDefined(typeof(DateRange), dateRangeValue)
            )
            {
                DateTime currentDate = DateTime.Today;
                switch ((DateRange)dateRangeValue)
                {
                    case DateRange.AllTime:
                        // No filtering needed
                        break;
                    case DateRange.Last7days:
                        orders = orders.Where(o => o.Createdat >= currentDate.AddDays(-7)).ToList();
                        break;
                    case DateRange.Last30days:
                        orders = orders
                            .Where(o => o.Createdat >= currentDate.AddDays(-30))
                            .ToList();
                        break;
                    case DateRange.CurrentMonth:
                        DateTime firstDayOfMonth = new DateTime(
                            currentDate.Year,
                            currentDate.Month,
                            1
                        );
                        orders = orders.Where(o => o.Createdat >= firstDayOfMonth).ToList();
                        break;
                }
            }

            // Custom date filter
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var from))
            {
                orders = orders.Where(o => o.Createdat >= from).ToList();
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var to))
            {
                //orders = orders.Where(o => o.Createdat <= to).ToList();

                // Extend toDate to include the full day
                var toEndOfDay = to.Date.AddDays(1).AddTicks(-1); // End of the day (23:59:59.999)
                orders = orders.Where(o => o.Createdat <= toEndOfDay).ToList();
            }

            // Apply sorting with sortBy and sortDirection
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "order":
                        orders =
                            sortDirection?.ToLower() == "asc"
                                ? orders.OrderBy(o => o.Orderid).ToList()
                                : orders.OrderByDescending(o => o.Orderid).ToList();
                        break;
                    case "date":
                        orders =
                            sortDirection?.ToLower() == "asc"
                                ? orders.OrderBy(o => o.Createdat).ToList()
                                : orders.OrderByDescending(o => o.Createdat).ToList();
                        break;
                    case "name":
                        orders =
                            sortDirection?.ToLower() == "asc"
                                ? orders.OrderBy(o => o.Customername).ToList()
                                : orders.OrderByDescending(o => o.Customername).ToList();
                        break;
                    case "amount":
                        orders =
                            sortDirection?.ToLower() == "asc"
                                ? orders.OrderBy(o => o.Totalamount).ToList()
                                : orders.OrderByDescending(o => o.Totalamount).ToList();
                        break;
                    default:
                        // Default sorting (e.g., by date descending)
                        orders = orders.OrderByDescending(o => o.Createdat).ToList();
                        break;
                }
            }
            else
            {
                // Default sorting if no sortBy specified
                orders = orders.OrderByDescending(o => o.Createdat).ToList();
            }
        }

        int totalItems = orders?.Count ?? 0;
        orders = orders?.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new OrdersHelperModelView
        {
            orders = orders,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
        };
    }

    public async Task<OrderDetailsHelperViewModel?> getOrderDetails(int orderId)
    {
        // order and customer data
        OrderDetailsHelperViewModel? order = await _orderRepository.GetOrderDetailsByOrderId(orderId);
        string completeDate = "";
        // completed at logic 
        if(order!=null && order.CompletedAt!=null)
        {
            TimeSpan dateDiff = (TimeSpan)(order.CompletedAt - order.Createdat); 
            completeDate = 
            (dateDiff.Days > 0 ? $"{dateDiff.Days} Days " : "")+
            (dateDiff.Hours > 0 ? $"{dateDiff.Hours} hours " : "")+
            (dateDiff.Minutes > 0 ? $"{dateDiff.Minutes} Minutes " : "");

            order.completedAtString = completeDate;
        }

        // feedback data
        Feedback? feedback = await _orderRepository.GetFeedbackByOrderId(orderId);
        if(feedback!=null && order!=null)
        {
            order.FeedbackComment = feedback.CommentsFeedback;
            order.ambienceRating = feedback.AmbienceRating ?? 0;
            order.foodRating = feedback.FoodRating ?? 0;
            order.serviceRating = feedback.ServiceRating ?? 0;
            order.Ratings = (feedback.AmbienceRating + feedback.FoodRating + feedback.ServiceRating) / 3;
        }


        List<OrdersTablesMapping>? ordersTablesMapping = await _orderRepository.GetTableByORderId(orderId);
        List<Repository.Models.Table>? tables = new();
        int sectionId = 0;
        if (order != null)
        {
            // table and section data
            if (ordersTablesMapping != null)
            {
                foreach (OrdersTablesMapping t in ordersTablesMapping)
                {
                    Repository.Models.Table? table = await _tableRepository.GetTablesById(t.Tableid);
                    if (table != null)
                    {
                        tables.Add(table);
                    }
                }
                sectionId = tables.Select(m => m.Sectionid).FirstOrDefault();
                Section? section = await _sectionRepository.GetSectionById(sectionId);

                if (section != null)
                {
                    order.Sectionid = sectionId;
                    order.Sectionname = section.Sectionname;
                }
                order.Tables = tables;
            }

            // items details
            List<OrderItemModifiersMappingViewModel>? oim =
                await _orderRepository.GetOIMByOrderId(orderId);

            List<ItemOrderViewModel> ItemAtOrder = new();

            // sub total
            decimal subtotal = 0;

            // list to handle processed items
            List<int> processedOrderItemMappingIds = new();
            foreach (OrderItemModifiersMappingViewModel itemid in oim)
            {
                if (processedOrderItemMappingIds.Contains(itemid.Orderitemmappingid))
                {
                    continue;
                }

                Item? item = await _itemRepository.GetItemById(itemid.itemId);
                if (item != null)
                {
                    // item name and all static details
                    ItemOrderViewModel itemOrderViewModel = new ItemOrderViewModel()
                    {
                        itemId = item.Itemid,
                        itemName = item.Itemname,
                        Rate = item.Rate,
                    };

                    itemOrderViewModel.totalQuantity = oim.Where(oim =>
                            oim.Orderitemmappingid == itemid.Orderitemmappingid
                        )
                        .Select(oim => oim.totalQuantity)
                        .FirstOrDefault();

                    // sub total
                    subtotal += ((item.Rate ?? 0) * (itemOrderViewModel.totalQuantity ?? 0));

                    // modifiers of items

                    List<int>? modifiersids = oim.Where(oim =>
                            oim.Orderitemmappingid == itemid.Orderitemmappingid
                        )
                        .Select(oim => oim.Modifierid)
                        .ToList();

                    List<ModifiersOrderViewModel>? ModifierOrder = new();
                    foreach (int m in modifiersids)
                    {
                        Modifier? modifier = await _modifierRepository.GetModifierById(m);
                        if (modifier != null)
                        {
                            ModifiersOrderViewModel modifiersOrderViewModel = new()
                            {
                                modifierId = modifier.Modifierid,
                                modifierName = modifier.Modifiername,
                                modifierRate = modifier.Modifierrate,
                            };
                            // sub total
                            subtotal += ((modifier.Modifierrate ?? 0) * (itemOrderViewModel.totalQuantity ?? 0));


                            ModifierOrder.Add(modifiersOrderViewModel);
                        }
                    }

                    itemOrderViewModel.ModifierOrder = ModifierOrder;
                    ItemAtOrder.Add(itemOrderViewModel);
                }
                // adding processed order+item mapping id into list
                processedOrderItemMappingIds.Add(itemid.Orderitemmappingid);
            }

            order.ItemAtOrder = ItemAtOrder;
            order.SubTotal = subtotal;
            Decimal Total = subtotal;

            // Tax amounts
            List<TaxAmountViewModel>? orderTaxMappings = await _orderRepository.GetTaxByOrderId(orderId);
            if(orderTaxMappings != null)
            {
                foreach(TaxAmountViewModel o in orderTaxMappings)
                {
                    TaxAndFee? taxAndFee = await _taxRepository.GetTaxByTaxId(o.TaxId);
                    if(taxAndFee!=null)
                    {
                        o.TaxName = taxAndFee.Taxname;
                        if(taxAndFee.Taxtype == 1) {
                            Total += ((o.TaxAmount * subtotal)/100);
                            o.TaxAmount = ((o.TaxAmount * subtotal)/100);
                        }else {
                            Total += o.TaxAmount;
                            o.TaxAmount = o.TaxAmount;
                        }
                        o.TaxAmount = Math.Round(o.TaxAmount, 2);
                    }
                }
            }

            order.TaxAmount = orderTaxMappings;
            order.Total = Math.Round(Total,2);
            return order;
        }
        else
            return null;
    }

    public async Task<byte[]> GenerateInvoicePdf(int orderId)
    {
        try{
            OrderDetailsHelperViewModel? order = await getOrderDetails(orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            using (var memoryStream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(memoryStream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A4);

                document.SetMargins(20, 30, 20, 30);
                PdfFont regularFont;
                PdfFont boldFont;
                try
                {
                    regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                    boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                }
                catch (Exception ex)
                {
                    regularFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);
                    boldFont = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                }

                // Create a table for the header with equal columns for logo and text
                iText.Layout.Element.Table headerTable = new iText.Layout.Element.Table(
                    UnitValue.CreatePercentArray(new float[] { 50, 50 })
                ); 

                // Add image to the left column, centered
                string imagePath = System.IO.Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "images",
                    "pizzashop_logo.png"
                ); 
                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        iText.Layout.Element.Image logo = new iText.Layout.Element.Image(ImageDataFactory.Create(imagePath))
                            .SetWidth(50)
                            .SetHorizontalAlignment(HorizontalAlignment.CENTER);
                        headerTable.AddCell(
                            new Cell()
                                .Add(logo)
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        );
                    }
                    catch (Exception ex)
                    {
                        headerTable.AddCell(
                            new Cell()
                                .Add(new Paragraph("Logo Missing"))
                                .SetBorder(Border.NO_BORDER)
                                .SetTextAlignment(TextAlignment.CENTER)
                                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        );
                    }
                }
                else
                {
                    headerTable.AddCell(
                        new Cell()
                            .Add(new Paragraph("Logo"))
                            .SetBorder(Border.NO_BORDER)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                    );
                }

                // Add text to the right column, centered
                Paragraph headerText = new Paragraph("PIZZASHOP")
                    .SetFontSize(20)
                    .SetBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(new DeviceRgb(5, 101, 166));
                headerTable.AddCell(
                    new Cell()
                        .Add(headerText)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                );

                // Center the entire header table
                headerTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                headerTable.SetMarginBottom(10); 

                // Adding header table to the document
                document.Add(headerTable);

                iText.Layout.Element.Table DetailsTable = new iText.Layout.Element.Table(
                    UnitValue.CreatePercentArray(new float[] { 50, 50 })
                )
                    .UseAllAvailableWidth()
                    .SetMarginTop(5);

                // Customer Details (left column)
                Div customerDiv = new Div();
                customerDiv.Add(
                    new Paragraph("Customer Details")
                        .SetFontSize(11)
                        .SetBold()
                        .SetFontColor(new DeviceRgb(5, 101, 166))
                        .SetMarginBottom(4)
                );
                customerDiv.Add(
                    new Paragraph($"Name: {order.Customername ?? "customer"}")
                        .SetFontSize(10)
                        .SetMarginTop(2)
                );
                customerDiv.Add(
                    new Paragraph($"Mob: {order.Customerphone}").SetFontSize(10).SetMarginTop(2)
                );
                DetailsTable.AddCell(
                    new Cell()
                        .Add(customerDiv)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetVerticalAlignment(VerticalAlignment.TOP)
                        .SetPaddingLeft(0)
                );

                // Order Details (right column)
                Div orderDiv = new Div();
                orderDiv.Add(
                    new Paragraph("Order Details")
                        .SetFontSize(11)
                        .SetBold()
                        .SetFontColor(new DeviceRgb(5, 101, 166))
                        .SetMarginBottom(4)
                );
                orderDiv.Add(
                    new Paragraph($"Invoice Number: {order.Orderid}").SetFontSize(10).SetMarginTop(2)
                );
                orderDiv.Add(
                    new Paragraph($"Date: {order.Createdat?.ToString("yyyy-MM-dd HH:mm:ss")}")
                        .SetFontSize(10)
                        .SetMarginTop(2)
                );
                orderDiv.Add(
                    new Paragraph($"Section: {order.Sectionname}")
                        .SetFontSize(10)
                        .SetMarginTop(2)
                );  

                string tablesString = "";

                if(order.Tables!=null)
                {
                    foreach(var i in order.Tables)
                    {
                        tablesString += i.Tablename + ",";
                    }
                }
                tablesString = tablesString.Substring(0, tablesString.Length - 1);
                

                orderDiv.Add(
                    new Paragraph($"Tables: {tablesString}")
                        .SetFontSize(10)
                        .SetMarginTop(2)
                );

                DetailsTable.AddCell(
                    new Cell()
                        .Add(orderDiv)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetVerticalAlignment(VerticalAlignment.TOP)
                        .SetPaddingLeft(0)
                );

                // Add the DetailsTable to the document
                document.Add(DetailsTable);

                // Items Table

                // Customer Color 
                regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                Color PizzaShopColor = new DeviceRgb(0, 102, 166);
                boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                

                iText.Layout.Element.Table ItemDetailsTable = new iText.Layout.Element.Table(5).UseAllAvailableWidth().SetMarginTop(10);

                    ItemDetailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Sr. No.")
                                    .SetFontColor(ColorConstants.WHITE)
                                    .SetBackgroundColor(PizzaShopColor))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMargin(0));

                    ItemDetailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Item")
                                    .SetFontColor(ColorConstants.WHITE)
                                    .SetBackgroundColor(PizzaShopColor))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMargin(0));

                    ItemDetailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Quantity")
                                    .SetFontColor(ColorConstants.WHITE)
                                    .SetBackgroundColor(PizzaShopColor))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMargin(0));

                    ItemDetailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Unit Price")
                                    .SetFontColor(ColorConstants.WHITE)
                                    .SetBackgroundColor(PizzaShopColor))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMargin(0));

                    ItemDetailsTable.AddHeaderCell(new Cell().Add(new Paragraph("Total")
                                    .SetFontColor(ColorConstants.WHITE)
                                    .SetBackgroundColor(PizzaShopColor))
                                    .SetBorder(Border.NO_BORDER)
                                    .SetPadding(0)
                                    .SetMargin(0)
                                    .SetTextAlignment(TextAlignment.LEFT));

                    if(order.ItemAtOrder!=null)
                    {
                        int count = 0;
                        foreach (ItemOrderViewModel o in order.ItemAtOrder)
                        {
                            ItemDetailsTable.AddCell(new Cell().Add(new Paragraph($"{++count}"))
                                            .SetPaddingTop(3)
                                            .SetPaddingLeft(10)
                                            .SetMargin(0)
                                            .SetBorder(Border.NO_BORDER)
                                            //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                            );

                            ItemDetailsTable.AddCell(new Cell()
                                            .Add(new Paragraph($"{o.itemName}"))
                                            .SetPadding(2).SetMargin(0)
                                            .SetBorder(Border.NO_BORDER)
                                            //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1)
                                            );
                        
                            ItemDetailsTable.AddCell(new Cell()
                                            .Add(new Paragraph($"{o.totalQuantity}"))
                                            .SetPadding(2).SetMargin(0)
                                            .SetBorder(Border.NO_BORDER)
                                            //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                            );
                                            
                            ItemDetailsTable.AddCell(new Cell()
                                            .Add(new Paragraph($"{o.Rate:f2}"))
                                            .SetPadding(2).SetMargin(0)
                                            .SetBorder(Border.NO_BORDER)
                                            //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                            );

                            ItemDetailsTable.AddCell(new Cell()
                                            .Add(new Paragraph($"{(o.totalQuantity*o.Rate):F2}"))
                                            .SetPadding(2)
                                            .SetPaddingRight(10)
                                            .SetMargin(0)
                                            .SetBorder(Border.NO_BORDER)
                                            //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                            .SetTextAlignment(TextAlignment.RIGHT)
                                            );

                            if(o.ModifierOrder!=null)
                            {
                                foreach (ModifiersOrderViewModel m in o.ModifierOrder)
                                {
                                    ItemDetailsTable.AddCell(new Cell()
                                                    .Add(new Paragraph())
                                                    .SetFontSize(10)
                                                    .SetPadding(1).SetMargin(0)
                                                    .SetBorder(Border.NO_BORDER)
                                                    //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1)
                                                    );
                                    
                                    ItemDetailsTable.AddCell(new Cell()
                                                    .Add(new Paragraph($" {m.modifierName}"))
                                                    .SetFontSize(10)
                                                    .SetPadding(1).SetMargin(0)
                                                    .SetBorder(Border.NO_BORDER)
                                                    //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1)
                                                    );
                                
                                    ItemDetailsTable.AddCell(new Cell()
                                                    .Add(new Paragraph($"{o.totalQuantity}"))
                                                    .SetFontSize(10)
                                                    .SetPadding(1).SetMargin(0)
                                                    .SetBorder(Border.NO_BORDER)
                                                    //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                                    );
                                                    
                                    ItemDetailsTable.AddCell(new Cell()
                                                    .Add(new Paragraph($"{m.modifierRate:F2}"))
                                                    .SetFontSize(10)
                                                    .SetPadding(1).SetMargin(0)
                                                    .SetBorder(Border.NO_BORDER)
                                                    //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                                    );

                                    ItemDetailsTable.AddCell(new Cell()
                                                    .Add(new Paragraph($"{(o.totalQuantity*m.modifierRate):F2}"))
                                                    .SetFontSize(10)
                                                    .SetPadding(1)
                                                    .SetPaddingRight(10)
                                                    .SetMargin(0)
                                                    .SetBorder(Border.NO_BORDER)
                                                    //.SetBorderBottom(new SolidBorder(PizzaShopColor, 1))
                                                    .SetTextAlignment(TextAlignment.RIGHT)
                                                    );
                                }
                            }

                            LineSeparator BlueLine = new LineSeparator(new SolidLine(1f)) 
                            .SetStrokeColor(new DeviceRgb(14, 110, 253))
                            .SetMarginBottom(5); 
                            ItemDetailsTable.AddCell(new Cell(1, 5)
                                .Add(BlueLine)
                                .SetBorder(Border.NO_BORDER));
                            
                        }
                    }
                    

                    document.Add(ItemDetailsTable);

                    // Total and tax calculation
                    iText.Layout.Element.Table TotalDetailsTable = new iText.Layout.Element.Table(2).UseAllAvailableWidth().SetMarginTop(20);

                            Cell subTotal = new Cell()
                                                    .Add(new Paragraph($"Sub Total : ").SetMarginBottom(1))
                                                    .SetFontSize(10)
                                                    .SetBorder(Border.NO_BORDER)
                                                    .SetTextAlignment(TextAlignment.LEFT);
                            TotalDetailsTable.AddCell(subTotal);

                            Cell subAmounts = new Cell()
                                                    .Add(new Paragraph($"{order.SubTotal:F2} ₹").SetMarginBottom(1))
                                                    .SetFontSize(10)
                                                    .SetBorder(Border.NO_BORDER)
                                                    .SetTextAlignment(TextAlignment.RIGHT);
                            TotalDetailsTable.AddCell(subAmounts);

                    if(order.TaxAmount!=null)
                    {
                        foreach (TaxAmountViewModel o in order.TaxAmount)
                        {
                            Cell TotalTitles = new Cell()
                                                    .Add(new Paragraph($"{o.TaxName} : ").SetMarginBottom(1))
                                                    .SetFontSize(10)
                                                    .SetBorder(Border.NO_BORDER)
                                                    .SetTextAlignment(TextAlignment.LEFT);
                            TotalDetailsTable.AddCell(TotalTitles);

                            Cell TotalAmounts = new Cell()
                                                    .Add(new Paragraph($"{o.TaxAmount:F2} ₹").SetMarginBottom(1))
                                                    .SetFontSize(10)
                                                    .SetBorder(Border.NO_BORDER)
                                                    .SetTextAlignment(TextAlignment.RIGHT);
                            TotalDetailsTable.AddCell(TotalAmounts);
                        }
                    }
                    document.Add(TotalDetailsTable);


                
                LineSeparator line = new LineSeparator(new SolidLine(1f)) 
                    .SetStrokeColor(new DeviceRgb(5, 101, 166))
                    .SetMarginBottom(5); 
                document.Add(line);
                iText.Layout.Element.Table AmountTable = new iText.Layout.Element.Table(
                    UnitValue.CreatePercentArray(new float[] { 50, 50 })
                ).UseAllAvailableWidth(); 

            
                Paragraph total = new Paragraph("Total Amount Due:")
                    .SetFontSize(12)
                    .SetBold()
                    .SetFontColor(new DeviceRgb(5, 101, 166))
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetMarginTop(10);
                AmountTable.AddCell(
                    new Cell()
                        .Add(total)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.LEFT) 
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetPaddingLeft(0)
                ); 

                // Total Amount Value (rightmost)
                Paragraph totalAmount = new Paragraph(@$"₹{order.Total:F2}")
                    .SetFontSize(12)
                    .SetBold()
                    .SetFontColor(new DeviceRgb(5, 101, 166))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetMarginTop(10);
                AmountTable.AddCell(
                    new Cell()
                        .Add(totalAmount)
                        .SetBorder(Border.NO_BORDER)
                        .SetTextAlignment(TextAlignment.RIGHT) 
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetPaddingRight(0)
                ); 

                document.Add(AmountTable);

                document.Add(
                    new Paragraph($"Payment Information")
                        .SetFontSize(11)
                        .SetFontColor(new DeviceRgb(5, 101, 166))
                        .SetMarginTop(5)
                );
                document.Add(
                    new Paragraph($"Payment Mode: {((PaymentMethod)order.Paymentmode)}").SetFontSize(10).SetMarginTop(4)
                );
        
                // Footer
                Paragraph footer = new Paragraph("THANK YOU!")
                    .SetFontSize(14)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(10);
                document.Add(footer);

                // Close document
                document.Close();

                return memoryStream.ToArray();
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log the error)
            throw new Exception("Error generating invoice PDF: " + ex.Message, ex);
        }
    }
}
