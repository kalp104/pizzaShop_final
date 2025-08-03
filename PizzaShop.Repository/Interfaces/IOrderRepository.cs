using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetOrderById(int orderid);
    Task<List<OrderItemMapping>?> GetAllOrderItemMappings();
    Task<List<Order>?> GetOrderByFilterDates(DateTime startDate, DateTime endDate);
    Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync();
    Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId);
    Task<Feedback?> GetFeedbackByOrderId(int orderid);
    Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid);
    Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid);
    Task<List<TaxAmountViewModel>?> GetTaxByOrderId(int OrderId);
    Task<List<OrderTax>> GetTaxesByOrderId(int orderId);
    Task<string> AddOrder(Order order);
    Task<string> AddOrderItemModifiersMapping(OrderItemModifiersMapping order);
    Task<string> AddOrdersCustomersMapping(OrdersCustomersMapping order);
    Task<string> AddOrdersTablesMapping(OrdersTablesMapping order);
    Task DeleteOrderItemMapping(OrderItemMapping orderItemMapping);
    Task<OrdersTablesMapping?> GetOrderByTableId(int tableid);
    Task<List<OrderItemModifiersMapping>> GetAllOrderItemModifiersMapping();
    Task<List<OrderItemMapping>> GetAllOrderItemMapping();
    Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin();
    Task<OrderItemMapping?> GetOrderItemMappingById(int OrderItemMappingId);
    Task<string> UpdateOrderItemMapping(OrderItemMapping order);
    Task<string> AddOrderItemMapping(OrderItemMapping order);
    Task<List<OrderItemMapping>?> GetOrderItemMappingByOrderId(int OrderId);
    Task<string> UpdateOrder(Order order);
    Task<string> UpdateOrderTaxMapping(OrderTaxMapping order);
    Task<string> AddOrderTaxMapping(OrderTaxMapping order);
    Task<string> DeleteOrderTaxMapping(OrderTaxMapping order);
    Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid);
    Task RemoveRangeOfOrderItemModifiersMappings(List<OrderItemModifiersMapping> mappings);
    Task<List<OrderTaxMapping>?> GetOrderTaxMappingByOrderId(int OrderId);


    // function for getting order item mapping by order id
    Task<List<OrderItemModifiersMappingViewModel>> FunctionGetOIMByOrderId(int orderid);

    Task<int> Fun_customer_Add_Edit_byId(string Customername,string Customeremail,decimal Customerphone,int userid);
    Task Sp_Delete_waitinglist(int Waitingid,int userid);
    
    // sp for updating order status 
    Task PsUpdateOrderStatus(int orderId, bool status);

    // sp for updating order_item_mapping's readyquanity and status by there mapping id
    Task Ps_UpdateOrderItemMapping_status_readyquantity(int OrderItemMappingId,int ReadyQuantity, int status);



    // sp
    Task<(int CustomerId, int OrderId)> CallSpAddCustomerAsync(
        string customerName, 
        string customerEmail, 
        decimal customerPhone, 
        int waitingId, 
        int totalPersons, 
        int totalAmount,  
        int userId);

    Task<List<WaitingListViewModel>> Function_GetAllWaitingList_OrderApp();
    Task<List<WaitingListViewModel>> Function_GetAllWaitingList_By_SectionId_OrderApp(int? sectionid = null);
    Task sp_UpdateOrderItemMapping(OrderItemMapping orderItemMapping);
    Task sp_AddFeedback(Feedback feedback);
    Task<bool> sp_EditWaitingListAsync(int waitingId, string cutomername, string customeremail,decimal customerphone, int totalPersons, int sectionid, int userId);
    Task<bool> Sp_Cancel_Complete_OrderHelper(int orderId, int OIstatus, int tableStatus, int orderStatus, int userId, decimal avgRating);
    Task<bool> Sp_EditCustomerDetails(
        int customerId, 
        string customerName, 
        string customerEmail, 
        decimal customerPhone,
        int orderId,
        int totalPersons, 
        int userId
    );
    Task<Order?> Function_GetOrderById(int orderid);

    Task<List<OrderItemMapping>?> Function_GetOrderItemMappingByOrderId(int orderid);

    Task<List<OrderItemModifiersMappingViewModel>> Function_GetOIMByOrderId(int orderid);

}
