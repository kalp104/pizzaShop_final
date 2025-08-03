using System;
using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Service.Implementations;

public class OrderAppService : IOrderAppService
{
    private readonly PizzaShop2Context _context;
    private readonly IOrderRepository _orderRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IModifierRepository _modifierRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IFeedBackRepository _feedbackRepository;

    public OrderAppService(
        IWebHostEnvironment webHostEnvironment,
        PizzaShop2Context context,
        IOrderRepository orderRepository,
        ISectionRepository sectionRepository,
        ITableRepository tableRepository,
        IModifierRepository modifierRepository,
        ITaxRepository taxRepository,
        IWaitingListRepository waitingListRepository,
        IItemRepository itemRepository,
        ICustomerRepository customerRepository,
        IFeedBackRepository feedbackRepository
    )
    {
        _context = context;
        _orderRepository = orderRepository;
        _sectionRepository = sectionRepository;
        _tableRepository = tableRepository;
        _modifierRepository = modifierRepository;
        _taxRepository = taxRepository;
        _waitingListRepository = waitingListRepository;
        _itemRepository = itemRepository;
        _customerRepository = customerRepository;
        _feedbackRepository = feedbackRepository;
    }



    public async Task<OrderAppKOTViewModel> GetCardDetails(int? category = null, int? status = null, bool? IsModal = false)
    {
        List<OrderKOTViewModel> orderKot = new(); 
        
        // calling function here
        List<OrderItemModifierJoinModelView> join = await _orderRepository.GetAllOrderItemModifierJoin();

        // all distinct orderids in the db
        List<int> orderIds = join.Select(u => u.orderId).Distinct().ToList();

            foreach (int orderId in orderIds)
            {
                OrderKOTViewModel orderKOTViewModel = new();

                // Fetch mappings by OrderId
                List<OrderItemModifiersMappingViewModel> mappings = await _orderRepository.FunctionGetOIMByOrderId(orderId);
                // Get all data of that order id
                // Order? order = await _orderRepository.GetOrderById(orderId);
                Order? order = await _orderRepository.Function_GetOrderById(orderId);
                // Fetch table and section details
                List<OrdersTablesMapping> ordersTablesMappings = await _orderRepository.GetTableByORderId(orderId);
                List<Table> tables = new();
                foreach (var t in ordersTablesMappings)
                {
                    Table? table = await _tableRepository.GetTablesById(t.Tableid);
                    if (table != null)
                    {
                        // adding table details based on order
                        tables.Add(table);
                    }
                }
                int sectionId = tables.Select(s => s.Sectionid).First();
                Section? s = await _sectionRepository.GetSectionById(sectionId);

                // adding details of tables and seciton, header & footer data
                if (order != null)
                {
                    orderKOTViewModel.orderId = order.Orderid;
                    orderKOTViewModel.Ordermessage = order.Ordermessage;
                    orderKOTViewModel.table = tables;
                    orderKOTViewModel.sectionName = s?.Sectionname;
                    orderKOTViewModel.ModalStatus = status ?? 1;
                }

                List<ItemsKOTViewModel> itemsKOT = new();
                // Track processed Orderitemmappingid
                List<int> processedOrderItemMappingIds = new();

                foreach (OrderItemModifiersMappingViewModel m in mappings)
                {
                    if (processedOrderItemMappingIds.Contains(m.Orderitemmappingid))
                    { // need to skip if that item already processed
                        continue;
                    }

                    ItemsKOTViewModel itemsKOTViewModel = new();

                    //fetch OrderItemMapping using Orderitemmappingid
                    OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(m.Orderitemmappingid);

                    if (orderItemMapping == null)
                    {
                        //if no matching OrderItemMapping found
                        continue;
                    }

                    // tetching Item details based on (order + item) mapping's mapping id
                    Item? item = await _itemRepository.GetItemById(orderItemMapping.Itemid);
                    if (item != null)
                    {
                        
                        itemsKOTViewModel.itemId = item.Itemid;
                        itemsKOTViewModel.OrderItemMappingId = orderItemMapping.Orderitemmappingid;
                        itemsKOTViewModel.itemName = item.Itemname;
                        itemsKOTViewModel.totalQuantity = orderItemMapping.Totalquantity;
                        itemsKOTViewModel.specialMessage = orderItemMapping.Specialmessage;
                        itemsKOTViewModel.status = orderItemMapping.Status;
                        itemsKOTViewModel.dateTime = orderItemMapping.Createdat.HasValue
                            ? orderItemMapping.Createdat.Value.Add(
                                DateTime.Now - orderItemMapping.Createdat.Value
                            )
                            : DateTime.Now;
                        itemsKOTViewModel.Readyquantity = orderItemMapping.Readyquantity;
                        // itemsKOTViewModel.Readyquantity = 0;


                        TimeSpan DateDifference = orderItemMapping.Createdat.HasValue 
                            ? (DateTime.Now - orderItemMapping.Createdat.Value) 
                            : TimeSpan.Zero;
                        string DateFormate = (DateDifference.Days > 0 ? $"{DateDifference.Days} Days " : "")
                                         +   (DateDifference.Hours > 0 ? $"{DateDifference.Hours} Hours " : "") 
				                         +   (DateDifference.Minutes > 0 ? $"{DateDifference.Minutes} Min " : ""); 
                        
                        itemsKOTViewModel.timeSpend = DateFormate;
                    }

                    //fetch modifiers for this Orderitemmappingid
                    List<OrderItemModifiersMappingViewModel> modifierMappings = mappings
                        .Where(o => o.Orderitemmappingid == m.Orderitemmappingid)
                        .ToList();
                    List<ModifiersKOTViewModel> ModifierKOT = new();
                    //modifiers based on (order+item+modifier) mapping
                    foreach (OrderItemModifiersMappingViewModel modifierMapping in modifierMappings)
                    {
                        ModifiersKOTViewModel modifiersKOTViewModel = new();
                        Modifier? modifier = await _modifierRepository.GetModifierById(modifierMapping.Modifierid);
                        if (modifier != null)
                        {
                            modifiersKOTViewModel.modifierId = modifier.Modifierid;
                            modifiersKOTViewModel.modifierName = modifier.Modifiername;
                        }
                        ModifierKOT.Add(modifiersKOTViewModel);
                    }
                    itemsKOTViewModel.ModifierKOT = ModifierKOT;

                    // Status base filtering cards
                    if (status == null)
                    {
                        // no status filter, add all items
                        itemsKOT.Add(itemsKOTViewModel);
                    }
                    else if (
                        status == (int)KOTStatus.InProgress
                        && itemsKOTViewModel.status == (int)KOTStatus.InProgress
                        && (
                            itemsKOTViewModel.Readyquantity == null
                            || itemsKOTViewModel.Readyquantity < itemsKOTViewModel.totalQuantity
                        )
                    )
                    {
                        //add if status is 1 and not all items are ready (in process)
                        itemsKOT.Add(itemsKOTViewModel);
                    }
                    else if (
                        status == (int)KOTStatus.Ready
                        && itemsKOTViewModel.status == (int)KOTStatus.InProgress
                        && itemsKOTViewModel.Readyquantity > 0
                    )
                    {
                        //add if status is 1 but some items are ready (in ready)
                        itemsKOT.Add(itemsKOTViewModel);
                    }
                    else if (
                        itemsKOTViewModel.status == status
                        && status == (int)KOTStatus.Ready
                        && itemsKOTViewModel.Readyquantity == itemsKOTViewModel.totalQuantity
                    )
                    {
                        //add if status is 2 and all items are ready (in ready)
                        itemsKOT.Add(itemsKOTViewModel);
                    }

                    processedOrderItemMappingIds.Add(m.Orderitemmappingid);
                }
                orderKOTViewModel.itemsKOT = itemsKOT;
                orderKot.Add(orderKOTViewModel);
            }

        OrderAppKOTViewModel result = new();
        List<OrderKOTViewModel> kot = new();

        // Category base filtering
        if (category != null)
        {
            foreach (OrderKOTViewModel i in orderKot)
            {
                if (i?.itemsKOT?.Count == 0)
                {
                    continue;
                }
                List<Item> items = await _itemRepository.GetItemsByCategoryId(category);
                List<int> itemIds = items.Select(i => i.Itemid).ToList();
                List<ItemsKOTViewModel> temp = i?.itemsKOT ?? new();
                if (temp != null)
                {
                    foreach (ItemsKOTViewModel t in temp)
                    {
                        if (itemIds.Contains(t.itemId) && i != null)
                        {
                            // adding cards for final result
                            kot.Add(i);
                            break;
                        }
                    }
                }
            }
            result.orderKOT = kot;
        }
        else
        {
            //if items are 0 in the model then no need to show card in the ui
            foreach (OrderKOTViewModel i in orderKot)
            {
                if (i?.itemsKOT?.Count == 0)
                {
                    continue;
                }
                if (i != null)
                { // if category is null
                    kot.Add(i);
                }
            }
            result.orderKOT = kot;
        }
        return result;
    }

    public async Task UpdateReadyQuantitiesAsync(int orderId,List<UpdateReadyQuantityModel> updates, int Status)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if(Status == 1)
            {
                bool status1 = true;
                foreach (UpdateReadyQuantityModel update in updates)
                {
                    OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(update.OrderItemMappingId);
                    if (orderItemMapping != null && orderItemMapping.Orderid == orderId) // Validate orderId
                    {
                        int status3 = 1;
                        int readyquantity3 = 0;
                        if (orderItemMapping.Readyquantity > update.ReadyQuantity)
                        {
                            status3 = (int)KOTStatus.InProgress;
                            status1 = true;
                        }
                        readyquantity3 = update.ReadyQuantity;
                        if (update.ReadyQuantity >= orderItemMapping.Totalquantity)
                        {
                            status3 = (int)KOTStatus.Ready;    
                            status1 = false;                       
                        }
                        await _orderRepository.Ps_UpdateOrderItemMapping_status_readyquantity(update.OrderItemMappingId,readyquantity3,status3);
                        await _orderRepository.PsUpdateOrderStatus(orderId,status1);    
                    }
                }
            }else {
                foreach (UpdateReadyQuantityModel update in updates)
                {
                    OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(update.OrderItemMappingId);
                    if (orderItemMapping != null && orderItemMapping.Orderid == orderId)
                    {
                        if (orderItemMapping.Readyquantity >= update.ReadyQuantity)
                        {
                            int status2 = (int)KOTStatus.InProgress; 
                            int ReadyQuantity = orderItemMapping.Readyquantity - update.ReadyQuantity ?? 0;
                            await _orderRepository.Ps_UpdateOrderItemMapping_status_readyquantity(update.OrderItemMappingId,ReadyQuantity,status2);
                            await _orderRepository.PsUpdateOrderStatus(orderId,true);    
                        }
                    }
                }
            } 
            await transaction.CommitAsync(); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateReadyQuantitiesAsync: {ex}");
        }
    }

    public async Task<List<WaitingListViewModel>> GetAllWaitingList()
    {
        // List<WaitingList>? waitingLists = await _waitingListRepository.GetAllWaitingLists();

        // List<WaitingListViewModel> waitingListViewModels = new List<WaitingListViewModel>();
        // if(waitingLists!=null)
        // {
        //     foreach (var waitingList in waitingLists)
        //     {
        //         WaitingListViewModel waitingListViewModel = new WaitingListViewModel
        //         {
        //             Waitingid = waitingList.Waitingid,
        //             Customername = waitingList.Customername,
        //             Customeremail = waitingList.Customeremail,
        //             Customerphone = waitingList.Customerphone,
        //             TotalPersons = waitingList.Totalperson,
        //             Sectionid = waitingList.Sectionid,
        //         };
        //         Section? s = await _sectionRepository.GetSectionById(waitingList.Sectionid);
        //         waitingListViewModel.Sectionname = s?.Sectionname;
        //         waitingListViewModels.Add(waitingListViewModel);
        //     }
        // }
        // return waitingListViewModels;
        List<WaitingListViewModel>? waitingLists = await _orderRepository.Function_GetAllWaitingList_OrderApp();
        return waitingLists;
    }

    public async Task<List<WaitingListViewModel>> GetAllWaitingListBySectionId(int? sectionId = null)
    {
        // List<WaitingList>? waitingLists = await _waitingListRepository.GetAllWaitingListsBySectionId(sectionId);

        // List<WaitingListViewModel> waitingListViewModels = new List<WaitingListViewModel>();
        // if(waitingLists!=null){
        //     foreach (var waitingList in waitingLists)
        //     {
        //         WaitingListViewModel waitingListViewModel = new WaitingListViewModel
        //         {
        //             Waitingid = waitingList.Waitingid,
        //             Customername = waitingList.Customername,
        //             Customeremail = waitingList.Customeremail,
        //             Customerphone = waitingList.Customerphone,
        //             TotalPersons = waitingList.Totalperson,
        //             Sectionid = waitingList.Sectionid,
        //         };
        //         Section? s = await _sectionRepository.GetSectionById(waitingList.Sectionid);
        //         waitingListViewModel.Sectionname = s?.Sectionname;
        //         waitingListViewModels.Add(waitingListViewModel);
        //     }
        // }
        // return waitingListViewModels;
        List<WaitingListViewModel>? waitingLists = await _orderRepository.Function_GetAllWaitingList_By_SectionId_OrderApp(sectionId);
        return waitingLists;
    }

    public async Task<OrderAppMenuViewModel?> AddCustomer(OrderAppTableViewModel model, int userid)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            
            int tableCapacity = 0;

            if(model.Tableids != null && model.Tableids.Count > 0)
            {
                foreach(int tableId in model.Tableids)
                {
                    Table? table = await _tableRepository.GetTablesById(tableId);
                    if (table != null)
                    {
                        tableCapacity += table.Capacity ?? 0;
                    }
                }  
                if(model.TotalPersons > tableCapacity)
                {
                    return null;
                }
            }
            
            
            // int customerid = await _orderRepository.Fun_customer_Add_Edit_byId(model.Customername,model.Customeremail,model.Customerphone ?? 0,userid);
            // if (customerid == 0)
            // {
            //     return null;
            // }

            // cheking for duplication
            // Customer? customers = await _customerRepository.CheckCustomerByEmail(model.Customeremail);
            // int customerid = 0;
            // if (customers != null)
            // {
            //     customers.Editedat = DateTime.Now;
            //     customers.Editedbyid = userid;
            //     customerid = customers.Customerid;
            //     await _customerRepository.UpdateCustomer(customers);
                
            // }
            // else{
            //     Customer customer = new Customer()
            //     {
            //         Customername = model.Customername,
            //         Customeremail = model.Customeremail,
            //         Customerphone = model.Customerphone ?? 0,
            //         Createdbyid = userid,
            //         Createdat = DateTime.Now,
            //         Editedat = DateTime.Now,
            //         Editedbyid = userid,
            //         Isdeleted = false,
            //     };  
            //     await _customerRepository.AddCustomer(customer);
            //     customerid = customer.Customerid;
            // }


            // // await _orderRepository.Sp_Delete_waitinglist(model.Waitingid,userid);
            // WaitingList? waitingList = await _waitingListRepository.GetWaitingListById(model.Waitingid);
            // if (waitingList != null)
            // {
            //     waitingList.Isdeleted = true;
            //     waitingList.Deletedat = DateTime.Now;
            //     waitingList.Deletedbyid = userid;
            //     await _waitingListRepository.UpdateWaitingList(waitingList);
            // }
            
            // Order order = new Order()
            // {
            //     Totalpersons = model.TotalPersons,
            //     Totalamount = 0,
            //     Ordertype = (int)Ordertype.Dinein,
            //     Paymentmode = 0,
            //     Status = (int)OrderStatus.Pending,
            //     Createdbyid = userid,
            //     Createdat = DateTime.Now,
            //     Editedat = DateTime.Now,
            //     Editedbyid = userid,
            //     Isdeleted = false,
            // };

            // await _orderRepository.AddOrder(order);
            // //
            // OrdersCustomersMapping ordersCustomersMapping = new OrdersCustomersMapping()
            // {
            //     Orderid = order.Orderid,
            //     Customerid = customerid,
            //     Createdbyid = userid,
            //     Createdat = DateTime.Now,
            //     Editedat = DateTime.Now,
            //     Editedbyid = userid,
            // };

            // await _orderRepository.AddOrdersCustomersMapping(ordersCustomersMapping);

            (int customerid1, int orderid1) = await _orderRepository.CallSpAddCustomerAsync(
                model.Customername,
                model.Customeremail,
                model.Customerphone ?? 0,
                model.Waitingid,
                model.TotalPersons ?? 0,
                0,
                userid
            );

            if (customerid1 == 0 || orderid1 == 0)
            {
                return null;
            }
        
            List<TableHelper> tableHelpers = new List<TableHelper>();


            if (model.Tableids != null && model.Tableids.Count > 0)
            {
                foreach (int tableId in model.Tableids)
                {
                    OrdersTablesMapping ordersTablesMapping = new OrdersTablesMapping()
                    {
                        Orderid = orderid1,
                        Tableid = tableId,
                        Createdbyid = userid,
                        Createdat = DateTime.Now,
                        Editedat = DateTime.Now,
                        Editedbyid = userid,
                    };
                    await _orderRepository.AddOrdersTablesMapping(ordersTablesMapping);

                    Table? table = await _tableRepository.GetTablesById(tableId);
                    if (table != null)
                    {
                        table.Status = (int)TableStatus.Assigned; // Assigned
                        table.Editedbyid = userid;
                        table.Editedat = DateTime.Now;
                        table.Isdeleted = false;
                        await _tableRepository.UpdateTable(table);
                    }

                    TableHelper tableHelper = new TableHelper()
                    {
                        Tableid = tableId,
                        Tablename = table?.Tablename ?? "",
                    };
                    tableHelpers.Add(tableHelper);
                }
            }else if(model?.Tableid != null)
            {
                OrdersTablesMapping ordersTablesMapping = new OrdersTablesMapping()
                {
                    Orderid = orderid1,
                    Tableid = model.Tableid,
                    Createdbyid = userid,
                    Createdat = DateTime.Now,
                    Editedat = DateTime.Now,
                    Editedbyid = userid,
                };
                await _orderRepository.AddOrdersTablesMapping(ordersTablesMapping);

                Table? table = await _tableRepository.GetTablesById(model.Tableid);
                if (table != null)
                {
                    table.Status = (int)TableStatus.Assigned; 
                    table.Editedbyid = userid;
                    table.Editedat = DateTime.Now;
                    table.Isdeleted = false;
                    await _tableRepository.UpdateTable(table);
                }

                TableHelper tableHelper = new TableHelper()
                {
                    Tableid = model.Tableid,
                    Tablename = table?.Tablename ?? "",
                };
                tableHelpers.Add(tableHelper);
            }

            await transaction.CommitAsync();

            int sectionId;
            sectionId = (model != null) ? model.Sectionid : 0;
            int tableid = tableHelpers.Select(t=>t.Tableid).FirstOrDefault();

            Section? section = await _sectionRepository.GetSectionById(sectionId);
            string sectionName = section?.Sectionname ?? "";            

            int orderId = orderid1;
            int customerId = customerid1;
            
            OrderPageViewModel orderPageViewModel = new OrderPageViewModel()
            {
                tableId = tableid,
                sectionId = sectionId,
                sectionName = sectionName,
                tableHelpers = tableHelpers,
                orderId = orderId,
                customerId = customerId,
            };
            OrderAppMenuViewModel obj = new OrderAppMenuViewModel()
            {
                orderPageViewModel = orderPageViewModel,
            };
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddCustomer: {ex}");
            throw new Exception("Error 500 : Internal server error");
        }
    }

    public async Task<OrderAppMenuViewModel> GetOrderPageDetailByTableId(int tableId)
    {
        try{
            OrdersTablesMapping? ordersTablesMapping = await _orderRepository.GetOrderByTableId(tableId);
            if (ordersTablesMapping == null)
            {
                return new OrderAppMenuViewModel();
            }

            Table? table = await _tableRepository.GetTablesById(ordersTablesMapping.Tableid);
            if (table == null)
            {
                return new OrderAppMenuViewModel();
            }

            Section? section = await _sectionRepository.GetSectionById(table.Sectionid);
            if (section == null)
            {
                return new OrderAppMenuViewModel();
            }

            int sectionId = section.Sectionid;
            string sectionName = section.Sectionname;

            int orderId = ordersTablesMapping.Orderid;
            OrderDetailsHelperViewModel? ordersCustomersMapping = await _orderRepository.GetOrderDetailsByOrderId(orderId);
            if (ordersCustomersMapping == null)
            {
                return new OrderAppMenuViewModel();
            }
            int customerId = ordersCustomersMapping.Customerid;

            List<TableHelper> tableHelpers = new List<TableHelper>();

            List<OrdersTablesMapping>? ordersTablesMappings = await _orderRepository.GetTableByORderId(orderId);
           if(ordersTablesMappings != null)
            {
                foreach (var t in ordersTablesMappings)
                {
                    Table? t1 = await _tableRepository.GetTablesById(t.Tableid);
                    if (t1 != null)
                    {
                        tableHelpers.Add(new TableHelper()
                        {
                            Tableid = t1.Tableid,
                            Tablename = t1.Tablename ?? "",
                        });
                    }
                }
            }


            List<TaxAndFee>? taxAndFees = await _taxRepository.GetAllTax();

            Customer? customer = await _customerRepository.GetCutomerById(customerId); 
            // Order? order = await _orderRepository.GetOrderById(orderId);
            Order? order = await _orderRepository.Function_GetOrderById(orderId);
            CustomerEditViewModel customers = new ()
            {
                Tableid = tableHelpers.First().Tableid,
                Orderid = order.Orderid,
                Customerid = customer.Customerid,
                Customername = customer.Customername,
                Customeremail = customer.Customeremail,
                Customerphone = customer.Customerphone,
                Totalperson = order?.Totalpersons ?? 0
            };


            OrderPageViewModel orderPageViewModel = new OrderPageViewModel()
            {
                sectionId = sectionId,
                sectionName = sectionName,
                tableHelpers = tableHelpers,
                orderId = orderId,
                customerId = customerId,
                taxAndFees = taxAndFees

            };
            Table? tableStatus = await _tableRepository.GetTablesById(tableId);
            OrderAppMenuViewModel obj = new OrderAppMenuViewModel()
            {
                orderPageViewModel = orderPageViewModel,
                customer = customers,
                Totalperson = order?.Totalpersons ?? 0,
                TableStatus = tableStatus?.Status ?? 0,
                Tableid = tableId,
            };
            return obj;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddCustomer: {ex}");
            return new OrderAppMenuViewModel();
        }
    }

    // for getting data if page get reload or the table status is 3
    public async Task<OrderDataViewModel> GetOrderItemDetails(int OrderId)
    {
        try
        { 
            // List<OrderItemMapping>? orderItemMappings = await _orderRepository.GetOrderItemMappingByOrderId(OrderId); 
            List<OrderItemMapping>? orderItemMappings = await _orderRepository.Function_GetOrderItemMappingByOrderId(OrderId); 
            List<OrderItem> orderItems = new ();
            OrderDataViewModel orderDataViewModel = new();

            // var order = await _orderRepository.GetOrderById(OrderId);
            var order = await _orderRepository.Function_GetOrderById(OrderId);
            if (order == null)
            {
                return orderDataViewModel; // Return empty model if order not found
            }

            // Populate order-level details
            orderDataViewModel.OrderId = OrderId;
            orderDataViewModel.OrderWiseComment = order.Ordermessage;
            orderDataViewModel.TotalAmount = order.Totalamount;
            orderDataViewModel.PaymentMode = order.Paymentmode;
           
            if(orderItemMappings != null)
            {
                foreach(OrderItemMapping i in orderItemMappings)
                {
                    OrderItem orderItem = new OrderItem();
                    Item? item = await _itemRepository.GetItemById(i.Itemid);
                    if(item != null)
                    {
                        orderItem.ItemId = item.Itemid;
                        orderItem.ItemName = item.Itemname;
                        orderItem.ItemRate = item.Rate ?? 0;
                        orderItem.TotalItems = (int)(i.Totalquantity ?? 0);
                        orderItem.Comment = i.Specialmessage;

                        // List<OrderItemModifiersMappingViewModel> mappings = await _orderRepository.GetOIMByOrderId(i.Orderid);
                        List<OrderItemModifiersMappingViewModel> mappings = await _orderRepository.Function_GetOIMByOrderId(i.Orderid);
                        List<OrderModifier> orderModifiers = new();
                        foreach (OrderItemModifiersMappingViewModel m in mappings)
                        {
                            if (m.Orderitemmappingid == i.Orderitemmappingid)
                            {
                                OrderModifier orderModifier = new OrderModifier();
                                Modifier? modifier = await _modifierRepository.GetModifierById(m.Modifierid);
                                if (modifier != null)
                                {
                                    orderModifier.ModifierId = modifier.Modifierid;
                                    orderModifier.ModifierName = modifier.Modifiername;
                                    orderModifier.ModifierRate = modifier.Modifierrate ?? 0;
                                    orderModifiers.Add(orderModifier);
                                }
                            }
                        }
                        orderItem.Modifiers = orderModifiers;
                        orderItems.Add(orderItem);
                    }
                }
                orderDataViewModel.Items = orderItems;
            }

            
            List<OrderTax> orderTaxes = new();
            List<OrderTax>? tax = await _orderRepository.GetTaxesByOrderId(OrderId);
            if(tax != null)
            {
                foreach(OrderTax i in tax)
                {
                    OrderTax orderTax = new OrderTax();
                    
                    if(tax != null)
                    {
                        orderTax.TaxId = i.TaxId;
                        orderTax.TaxAmount = i.TaxAmount;
                        orderTax.TaxType = i.TaxType; ;
                        orderTax.Isenabled = i.Isenabled;
                        orderTaxes.Add(orderTax);
                    }
                }
                orderDataViewModel.OrderId = OrderId;
                orderDataViewModel.Taxes = orderTaxes;
            }
            return orderDataViewModel;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error in GetOrderItemDetails: {ex}");
            return new OrderDataViewModel();
        }
    }

    public async Task<OrderAppWaitingTokenViewModel> GetWaitingTokens(int? sectionId = null)
    {
        List<Section>? sections = await _sectionRepository.GetAllSections();
        List<WaitingList>? waitingListWithDeletedValue = await _waitingListRepository.GetAllWaitingLists();
       
        List<WaitingListViewModel> waitingListViewModels = new List<WaitingListViewModel>();
        if(waitingListWithDeletedValue != null)
        {
            foreach (WaitingList waitingList in waitingListWithDeletedValue)
            {
                WaitingListViewModel waitingListViewModel = new WaitingListViewModel
                {
                    Waitingid = waitingList.Waitingid,
                    Customername = waitingList.Customername,
                    Customeremail = waitingList.Customeremail,
                    Customerphone = waitingList.Customerphone,
                    TotalPersons = waitingList.Totalperson,
                    Sectionid = waitingList.Sectionid,
                    createdAt = waitingList.Createdat,
                };
                Section? s = await _sectionRepository.GetSectionById(waitingList.Sectionid);
                waitingListViewModel.Sectionname = s?.Sectionname;
                waitingListViewModels.Add(waitingListViewModel);
            }
        }
        

        List<SectionWatitingTokenViewModel> SWT = new();
        if(sections !=  null){
            foreach (Section section in sections)
            {
                SectionWatitingTokenViewModel sectionWatitingTokenViewModel =
                    new SectionWatitingTokenViewModel()
                    {
                        Sectionid = section.Sectionid,
                        Sectionname = section.Sectionname,
                        WaitingListCount = waitingListWithDeletedValue != null?  waitingListWithDeletedValue.Count(w => w.Sectionid == section.Sectionid) : 0,
                    };
                SWT.Add(sectionWatitingTokenViewModel);
            }
        }
        
        if (sectionId != null)
        {
            waitingListViewModels = waitingListViewModels
                .Where(s => s.Sectionid == sectionId)
                .ToList();
        }
        return new OrderAppWaitingTokenViewModel()
        {
            sections = SWT,
            waitingLists = waitingListViewModels,
        };
    }

    public async Task<OrderAppWaitingTokenViewModel> GetCustomerDetails(string email)
    {
        email = email.ToLower();

        List<Customer>? customers = await _customerRepository.GetCustomerByEmail(email);

        if (customers == null || customers.Count == 0)
        {
            return new OrderAppWaitingTokenViewModel() { found = false };
        }

        return new OrderAppWaitingTokenViewModel()
        {
            customers = customers.Take(3).ToList(),
            found = true,
        };
    }

    public async Task<bool> DeleteWaitingToken(int waitingId, int userId)
    {
        try
        {
            WaitingList? waitingList = await _waitingListRepository.GetWaitingListById(waitingId);
            if (waitingList != null)
            {
                waitingList.Isdeleted = true;
                waitingList.Deletedat = DateTime.Now;
                waitingList.Deletedbyid = userId;
                await _waitingListRepository.UpdateWaitingList(waitingList);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteWaitingToken: {ex}");
            return false;
        }
    }

    public async Task<bool> EditWaitingToken(OrderAppTableViewModel model, int userid)
    {
        try
        {
            // WaitingList? waitingList = await _waitingListRepository.GetWaitingListById(model.Waitingid);
            // if (waitingList != null)
            // {
            //     waitingList.Customername = model.Customername;
            //     waitingList.Customeremail = model.Customeremail;
            //     waitingList.Customerphone = model.Customerphone ?? 0;
            //     waitingList.Totalperson = model.TotalPersons ?? 0;
            //     waitingList.Sectionid = model.Sectionid;
            //     waitingList.Editedbyid = userid;
            //     waitingList.Editedat = DateTime.Now;

            //     await _waitingListRepository.UpdateWaitingList(waitingList);
            // }
            // else
            // {
            //     return false;
            // }
            // return true;

            return await _orderRepository.sp_EditWaitingListAsync(
                model.Waitingid,
                model.Customername,
                model.Customeremail,
                model.Customerphone ?? 0,
                model.TotalPersons ?? 0,
                model.Sectionid,
                userid
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in EditWaitingToken: {ex}");
            return false;
        }
    }


    public async Task<List<Table>?> GetTablesBySectionId(int? sectionId = null, int? capacity = null)
    {
        try
        {
            List<Table>? tables = await _tableRepository.GetTableBySectionIdAsync(sectionId);
            tables = tables.Where( t => t.Status == 1 && t.Capacity >= capacity).ToList();
            return tables;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTablesBySectionId: {ex}");
            return new List<Table>();
        }
    }


    public async Task<bool> EditCustomerDetails(CustomerEditViewModel model,int userid)
    {
        try{
            List<OrdersTablesMapping>? table = await _orderRepository.GetTableByORderId(model.Orderid);
            if(table!=null)
            {
                int capacity = 0;
                foreach(OrdersTablesMapping tab in table)
                {
                    Table? tempTable = await _tableRepository.GetTablesById(tab.Tableid);
                    if(tempTable!=null) {
                        capacity += tempTable.Capacity ?? 0;
                    }
                }
                if(capacity < model.Totalperson)
                {
                    return false;
                }
            }
            
            bool res = await _orderRepository.Sp_EditCustomerDetails(
                model.Customerid,
                model.Customername,
                model.Customeremail,
                model.Customerphone,
                model.Orderid,
                model.Totalperson,
                userid
            );
            // Customer? customer = await _customerRepository.GetCutomerById(model.Customerid);
            // if(customer!=null)
            // {
            //     customer.Customername = model.Customername;
            //     customer.Customerphone = model.Customerphone;
            //     customer.Customeremail = model.Customeremail;
            //     customer.Editedat = DateTime.Now;
            //     customer.Editedbyid = userid;
            //     await _customerRepository.UpdateCustomer(customer);
            // }
            // Order? order = await _orderRepository.GetOrderById(model.Orderid);
            // if(order != null)
            // {
            //     order.Totalpersons = model.Totalperson;
            //     await _orderRepository.UpdateOrder(order);
            // } 
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error 500 : internal server error!");
        }    
    }


    public async Task<bool> AddOrderDetails(OrderDataViewModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (model == null || model.Items == null || model.Taxes == null)
            {
                Console.WriteLine("Model, Items, or Taxes is null");
                return false;
            }

            // Fetch order from order id
            // Order? order = await _orderRepository.GetOrderById(model.OrderId);
            Order? order = await _orderRepository.Function_GetOrderById(model.OrderId);
            if (order == null)
            {
                Console.WriteLine($"Order not found for OrderId: {model.OrderId}");
                return false;
            }

            // Fetch existing order item mappings and modifiers
            // List<OrderItemMapping>? orderItemMappings = await _orderRepository.GetOrderItemMappingByOrderId(model.OrderId);
            List<OrderItemMapping>? orderItemMappings = await _orderRepository.Function_GetOrderItemMappingByOrderId(model.OrderId);
            // List<OrderItemModifiersMappingViewModel> mappings = await _orderRepository.GetOIMByOrderId(model.OrderId);
            List<OrderItemModifiersMappingViewModel> mappings = await _orderRepository.Function_GetOIMByOrderId(model.OrderId);
            string notDeletedItems = ""; 
            // Delete OrderItemMappings not in incoming items (considering ModifierIds)
            if (orderItemMappings != null)
            {
                List<OrderItemMapping> mappingsToDelete = new();
                foreach (OrderItemMapping existingMapping in orderItemMappings)
                {
                    // getting OrderItem for ModifierChecker
                    List<int> existingModifierIds = mappings
                        .Where(m => m.Orderitemmappingid == existingMapping.Orderitemmappingid)
                        .Select(m => m.Modifierid)
                        .ToList();

                    OrderItem pseudoItem = new OrderItem
                    {
                        ItemId = existingMapping.Itemid,
                        Modifiers = existingModifierIds.Select(id => new OrderModifier { ModifierId = id }).ToList()
                    };

                    // Check if any incoming item matches this ItemId and ModifierId set
                    bool hasMatch = model.Items.Any(incomingItem =>
                        incomingItem.ItemId == existingMapping.Itemid &&
                        incomingItem.Modifiers != null &&
                        incomingItem.Modifiers.Select(m => m.ModifierId).OrderBy(id => id)
                            .SequenceEqual(existingModifierIds.OrderBy(id => id)));

                    if (!hasMatch)
                    {
                        mappingsToDelete.Add(existingMapping);
                    }
                }

                //if some of the items are got ready, they cannot get delete
                
                foreach (OrderItemMapping mapping in mappingsToDelete)
                {
                    if((mapping.Status > 1) || (mapping.Readyquantity > 0))
                    {
                        Item? i = await _itemRepository.GetItemById(mapping.Itemid);
                        notDeletedItems += i?.Itemname?.ToString() + ", ";
                    }
                    else
                    {
                        // Delete associated OrderItemModifiersMapping entries
                        List<OrderItemModifiersMapping> modifierMappings = await _orderRepository.GetOIMByOrderItemMappingId(mapping.Orderitemmappingid);
                        if (modifierMappings.Any())
                        {
                            await _orderRepository.RemoveRangeOfOrderItemModifiersMappings(modifierMappings);
                        }

                        // Delete OrderItemMapping
                        await _orderRepository.DeleteOrderItemMapping(mapping);
                    }  
                }
            }

            

            // Update order details
            order.Ordermessage = model.OrderWiseComment;
            order.Paymentmode = model.PaymentMode;
            order.Totalamount = Math.Round(model.TotalAmount,2);
            order.Status = 3;
            await _orderRepository.UpdateOrder(order);

            // Update table details status (assigned to running)
            List<OrdersTablesMapping>? ordersTablesMapping = await _orderRepository.GetTableByORderId(model.OrderId);
            if (ordersTablesMapping != null)
            {
                foreach (OrdersTablesMapping i in ordersTablesMapping)
                {
                    Table? table = await _tableRepository.GetTablesById(i.Tableid);
                    if (table != null)
                    {
                        table.Status = 3; // Running table
                        await _tableRepository.UpdateTable(table);
                    }
                }
            }


            // Process each incoming item
            foreach (OrderItem incomingItem in model.Items)
            {
                if (incomingItem == null)
                {
                    Console.WriteLine("Incoming item is null");
                    continue;
                }

                // Get incoming ModifierIds
                List<int> incomingModifierIds = incomingItem.Modifiers?.Select(m => m.ModifierId).OrderBy(id => id).ToList() ?? new List<int>();

                // Find existing OrderItemMapping with matching ItemId and ModifierIds
                OrderItemMapping? existingMapping = null;
                if (orderItemMappings != null)
                {
                    foreach (OrderItemMapping mapping in orderItemMappings)
                    {
                        List<int> existingModifierIds = mappings
                            .Where(m => m.Orderitemmappingid == mapping.Orderitemmappingid)
                            .Select(m => m.Modifierid)
                            .OrderBy(id => id)
                            .ToList();
                        if (mapping.Itemid == incomingItem.ItemId &&
                            incomingModifierIds.SequenceEqual(existingModifierIds))
                        {
                            existingMapping = mapping;
                            break;
                        }
                    }
                }
                if (existingMapping != null)
                {
                    // Update existing OrderItemMapping
                    if(existingMapping.Totalquantity > incomingItem.TotalItems)
                    {   
                        OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(existingMapping.Orderitemmappingid);
                        
                        if((orderItemMapping.Status == 2 
                            || orderItemMapping.Readyquantity > incomingItem.TotalItems) && orderItemMapping!=null)
                        {
                            throw new Exception("Items are ready already! please check the KOT");  
                        }
                    }
                    if(existingMapping.Readyquantity == incomingItem.TotalItems )
                    {
                        existingMapping.Status = 2; // Mark as Ready
                    }
                    else if(existingMapping.Readyquantity < incomingItem.TotalItems && existingMapping.Totalquantity == existingMapping.Readyquantity)
                    {
                        existingMapping.Status = 1; // Mark as In Progress
                    }
                     
                    existingMapping.Totalquantity = incomingItem.TotalItems;
                    existingMapping.Specialmessage = incomingItem.Comment;
 
                    await _orderRepository.sp_UpdateOrderItemMapping(existingMapping);
                }
                else
                {
                    // Add new OrderItemMapping
                    // Console.WriteLine($"Adding new OrderItemMapping: ItemId={incomingItem.ItemId}, Modifiers={string.Join(",", incomingModifierIds)}");
                    OrderItemMapping orderItemMapping = new()
                    {
                        Orderid = model.OrderId,
                        Itemid = incomingItem.ItemId,
                        Totalquantity = incomingItem.TotalItems,
                        Specialmessage = incomingItem.Comment,
                        Status = 1, // In progress
                        Createdat = DateTime.Now
                    };
                    await _orderRepository.AddOrderItemMapping(orderItemMapping);

                    // Add OrderItemModifiersMapping entries
                    if (incomingItem.Modifiers != null)
                    {
                        foreach (OrderModifier modifier in incomingItem.Modifiers)
                        {
                            if (modifier == null)
                            {
                                Console.WriteLine("Modifier is null");
                                continue;
                            }
                            OrderItemModifiersMapping mapping = new()
                            {
                                Modifierid = modifier.ModifierId,
                                Orderitemmappingid = orderItemMapping.Orderitemmappingid,
                                Createdat = DateTime.Now
                            };
                            await _orderRepository.AddOrderItemModifiersMapping(mapping);
                        }
                    }
                }
            }

            // Fetch existing order tax mappings
            List<OrderTaxMapping>? orderTaxMappings = await _orderRepository.GetOrderTaxMappingByOrderId(model.OrderId);
            List<int> existingTaxIds = orderTaxMappings?.Select(i => i.Taxid).ToList() ?? new List<int>();

            // Process taxes based on isChecked
            foreach (OrderTax tax in model.Taxes)
            {
                if (tax == null)
                {
                    // Console.WriteLine("Tax is null");
                    continue;
                }

                if (tax.isChecked == true)
                {
                    // Add or update tax if checked
                    if (existingTaxIds.Contains(tax.TaxId))
                    {
                        // Update existing OrderTaxMapping
                        OrderTaxMapping? existingTax = orderTaxMappings?.FirstOrDefault(o => o.Taxid == tax.TaxId);
                        if (existingTax != null)
                        {
                            existingTax.Totalamount = tax.TaxAmount;
                            existingTax.Editedat = DateTime.Now;
                            await _orderRepository.UpdateOrderTaxMapping(existingTax);
                        }
                    }
                    else
                    {
                        // Add new OrderTaxMapping
                        OrderTaxMapping orderTaxMapping = new()
                        {
                            Orderid = model.OrderId,
                            Taxid = tax.TaxId,
                            Totalamount = tax.TaxAmount,
                            Createdat = DateTime.Now,
                            Editedat = DateTime.Now
                        };
                        await _orderRepository.AddOrderTaxMapping(orderTaxMapping);
                    }
                }
                else if (existingTaxIds.Contains(tax.TaxId))
                {
                    // Delete tax if unchecked
                    OrderTaxMapping? existingTax = orderTaxMappings?.FirstOrDefault(o => o.Taxid == tax.TaxId);
                    if (existingTax != null)
                    {
                        await _orderRepository.DeleteOrderTaxMapping(existingTax);
                    }
                }
            }

            // Delete any OrderTaxMappings not in model.Taxes
            List<int> incomingTaxIds = model.Taxes.Where(t => t != null).Select(t => t.TaxId).ToList();
            List<OrderTaxMapping>? taxesToDelete = orderTaxMappings?.Where(t => !incomingTaxIds.Contains(t.Taxid)).ToList();
            if (taxesToDelete != null)
            {
                foreach (OrderTaxMapping tax in taxesToDelete)
                {
                    await _orderRepository.DeleteOrderTaxMapping(tax);
                }
            }

            
            
            if (string.IsNullOrEmpty(notDeletedItems))
            {
                await transaction.CommitAsync();
                return true; 
            }
            else
            {   
                notDeletedItems = notDeletedItems.Substring(0, notDeletedItems.Length-2);
                throw new Exception(notDeletedItems + " are ready already!");
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<bool> CheckStateOfItems(int orderid)
    {
        try{
            // List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.GetOIMByOrderId(orderid);
            List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.Function_GetOIMByOrderId(orderid);
            if(order!=null)
            {
                foreach(OrderItemModifiersMappingViewModel o in order)
                {
                    if(o.status == (int)KOTStatus.InProgress){
                        return false;
                    }
                }
            }
            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<int> CompleteOrder(int orderid,int userid, FeedbackViewModel feedback)
    {
        try{
            await using var transaction = await _context.Database.BeginTransactionAsync();
            decimal avgRating = 0; 
            if(feedback!=null) 
            {
                avgRating = (feedback.ambienceRating+feedback.serviceRating+feedback.foodRating)/3;
                
                Feedback feedback1 = new (){
                    FoodRating = feedback.foodRating,
                    Orderid = orderid,
                    AmbienceRating = feedback.ambienceRating,
                    ServiceRating = feedback.serviceRating,
                    CommentsFeedback = feedback.comment,
                    Createdat = DateTime.Now,
                    Createdbyid = userid,
                    Editedbyid = userid,
                    Editedat = DateTime.Now
                };

                await _orderRepository.sp_AddFeedback(feedback1);
            }


            // List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.GetOIMByOrderId(orderid);
            List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.Function_GetOIMByOrderId(orderid);
            if(order!=null)
            {
                foreach(OrderItemModifiersMappingViewModel o in order)
                {
                    if(o.status == (int)KOTStatus.InProgress){
                        return 2;
                    }
                }

                // foreach(OrderItemModifiersMappingViewModel o in order)
                // {
                //     if(o.status == (int)KOTStatus.Ready){
                //         OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(o.Orderitemmappingid);
                //         if(orderItemMapping!=null)
                //         {
                //             orderItemMapping.Status = (int)KOTStatus.CompleteOrCancel;
                //             await _orderRepository.sp_UpdateOrderItemMapping(orderItemMapping);
                //         }
                //     }
                // }

                // // updating table details status (assigned to running)
                // List<OrdersTablesMapping>? ordersTablesMapping = await _orderRepository.GetTableByORderId(orderid);
                // if(ordersTablesMapping != null)
                // {
                //     foreach(OrdersTablesMapping i in ordersTablesMapping)
                //     {
                //         Table? table = await _tableRepository.GetTablesById(i.Tableid);
                //         if(table!=null)
                //         {
                            
                //             table.Status = (int)TableStatus.Available;
                //             await _tableRepository.UpdateTable(table);
                //         }
                //     }
                // }

                // Order? order1 = await _orderRepository.GetOrderById(orderid);
                // if(order1!=null)
                // {
                //     order1.Status = (int)OrderStatus.Completed;
                //     order1.Ratings = avgRating; 
                //     order1.Completedat = DateTime.Now;
                //     order1.Editedat = DateTime.Now;
                //     await _orderRepository.UpdateOrder(order1);
                // }

                bool res = await _orderRepository.Sp_Cancel_Complete_OrderHelper(orderid, (int)KOTStatus.CompleteOrCancel, (int)TableStatus.Available, (int)OrderStatus.Completed, userid, avgRating);
                await transaction.CommitAsync();
            }
            // can succefully complete the order
            return 1;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return 2;
        }
        
    }

    public async Task<bool> CancelOrder(int orderid, int userid)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.GetOIMByOrderId(orderid);
            List<OrderItemModifiersMappingViewModel>? order = await _orderRepository.Function_GetOIMByOrderId(orderid);
            if(order!=null)
            {
                foreach(OrderItemModifiersMappingViewModel o in order){
                    if(o.status == (int)KOTStatus.Ready || (o.ReadyQuantity < o.totalQuantity && o.ReadyQuantity!=0) ) return false;
                }

                bool res = await _orderRepository.Sp_Cancel_Complete_OrderHelper(orderid, (int)KOTStatus.CompleteOrCancel, (int)TableStatus.Available, (int)OrderStatus.Cancelled, userid, 0);
                // foreach(OrderItemModifiersMappingViewModel o in order)
                // {
                //     if(o.status == (int)KOTStatus.InProgress){
                //         OrderItemMapping? orderItemMapping = await _orderRepository.GetOrderItemMappingById(o.Orderitemmappingid);
                //         if(orderItemMapping!=null)
                //         {
                //             orderItemMapping.Status = (int)KOTStatus.CompleteOrCancel; // not in ready not in progress (in complete or canceled)
                //             await _orderRepository.sp_UpdateOrderItemMapping(orderItemMapping);
                //         }
                //     }
                // }
                // List<OrdersTablesMapping>? ordersTablesMapping = await _orderRepository.GetTableByORderId(orderid);
                // if(ordersTablesMapping != null)
                // {
                //     foreach(OrdersTablesMapping i in ordersTablesMapping)
                //     {
                //         Table? table = await _tableRepository.GetTablesById(i.Tableid);
                //         if(table!=null)
                //         {   
                //             table.Status = (int)TableStatus.Available;
                //             await _tableRepository.UpdateTable(table);
                //         }
                //     }
                // }
                // Order? order1 = await _orderRepository.GetOrderById(orderid);
                // if(order1!=null)
                // {
                //     order1.Status = (int)OrderStatus.Cancelled;
                //     order1.Editedat = DateTime.Now;
                //     order1.Editedbyid = userid;
                //     await _orderRepository.UpdateOrder(order1);
                // }  
                
                await transaction.CommitAsync();
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }

    public async Task<bool> AddFavourite(int itemid)
    {
        try{
            Item? item = await _itemRepository.GetItemById(itemid);
            if(item==null)return false;

            item.Favourite = !item.Favourite;
            await _itemRepository.UpdateItem(item); 

            return true;
        }
        catch (Exception e){
            Console.WriteLine(e.Message);
            return false;
        }
    }
   
}
