using System;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PizzaShop2Context _context;
    private IDbConnection _dbConnection { get; }

    public OrderRepository(PizzaShop2Context context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;
    }


    public async Task<Order?> GetOrderById(int orderid)
    {
        return await _context.Orders
                     .Where(x => x.Orderid == orderid)
                     .FirstOrDefaultAsync();
    }

    public async Task<List<OrderItemMapping>?> GetAllOrderItemMappings()
    {
        return await _context.OrderItemMappings
                     .ToListAsync();
    }

    public async Task<List<Order>?> GetOrderByFilterDates(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
                     .Where(o => o.Createdat >= startDate && o.Createdat <= endDate)
                     .ToListAsync();
    }


    public async Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync()
    {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid into ordersGroup
                from o in ordersGroup.DefaultIfEmpty()
                where c.Isdeleted == false
                where o == null || o.Isdeleted == false
                select new OrderCutstomerViewModel
                {
                    Orderid = o != null ? o.Orderid : 0,
                    Customerid = c.Customerid,
                    Orderdescription = o != null ? o.Orderdescription : null,
                    Createdat = o != null ? o.Createdat : null,
                    Status = o != null ? o.Status : 0,
                    Paymentmode = o != null ? o.Paymentmode : 0,
                    Ratings = o != null ? o.Ratings : null,
                    Totalamount = o != null ? o.Totalamount : 0,
                    Customername = c.Customername,
                    Customeremail = c.Customeremail,
                    Customerphone = c.Customerphone,
                }
            ).ToListAsync();
            return result;
    }

    public async Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId)
    {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid
                where c.Isdeleted == false
                where o.Isdeleted == false
                where o.Orderid == orderId
                orderby o.Createdat descending
                select new OrderDetailsHelperViewModel
                {
                    Orderid = o.Orderid,
                    Customerid = c.Customerid,
                    Orderdescription = o.Orderdescription,
                    Createdat = o.Createdat,
                    CompletedAt = o.Completedat,
                    Status = o.Status,
                    Paymentmode = o.Paymentmode,
                    Ratings = o.Ratings,
                    Totalamount = o.Totalamount,
                    Customername = c.Customername,
                    Customeremail = c.Customeremail,
                    Customerphone = c.Customerphone,
                    Totalpersons = o.Totalpersons,
                }
            ).FirstOrDefaultAsync();
            return result;
    }


    public async Task<Feedback?> GetFeedbackByOrderId(int orderid)
    {
        var result = await _context.Feedbacks
                .Where(f => f.Orderid == orderid)
                .FirstOrDefaultAsync();

        return result;
    }

    public async Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid)
    {
        return await _context.OrdersTablesMappings.Where(o => o.Orderid == orderid).ToListAsync();
    }

    public async Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid)
    {
        List<OrderItemModifiersMappingViewModel> result = await (
                from oi in _context.OrderItemMappings
                join oim in _context.OrderItemModifiersMappings
                on oi.Orderitemmappingid equals oim.Orderitemmappingid
                where oi.Orderid == orderid
                select new OrderItemModifiersMappingViewModel
                {
                    Mappingid = oim.Mappingid,
                    Orderitemmappingid = oi.Orderitemmappingid,
                    orderId = oi.Orderid,
                    itemId = oi.Itemid,
                    Modifierid = oim.Modifierid,
                    status = oi.Status ?? 0,
                    totalQuantity = oi.Totalquantity ?? 0,
                    ReadyQuantity = oi.Readyquantity ?? 0

                }
        ).ToListAsync();
        return result;
    }

    public async Task<List<TaxAmountViewModel>?> GetTaxByOrderId(int OrderId)
    {
        List<TaxAmountViewModel>? result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId 
                select new TaxAmountViewModel
                {
                    TaxId = ot.Taxid,
                    TaxAmount = ot.Totalamount
                }
        ).ToListAsync();

        return result;
    }



    public async Task<string> AddOrder(Order order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> AddOrdersTablesMapping(OrdersTablesMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddOrdersCustomersMapping(OrdersCustomersMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<List<OrderTax>> GetTaxesByOrderId(int orderId){
            var result = await (
                from ot in _context.OrderTaxMappings
                join t in _context.TaxAndFees on ot.Taxid equals t.Taxid
                where ot.Orderid == orderId
                select new OrderTax
                {
                    TaxId = t.Taxid,
                    TaxName = t.Taxname,
                    TaxType = t.Taxtype,
                    TaxAmount = t.Taxamount,
                    Isenabled = t.Isenabled
                }
            ).ToListAsync();

            return result;
        }

    public async Task<OrdersTablesMapping?> GetOrderByTableId(int tableid)
    {
        OrdersTablesMapping? result = await (
                from ot in _context.OrdersTablesMappings
                join o in _context.Orders on ot.Orderid equals o.Orderid
                where ot.Tableid == tableid && o.Isdeleted == false
                orderby o.Createdat descending
                select new OrdersTablesMapping
                {
                    Orderid = o.Orderid,
                    Tableid = ot.Tableid
                }
        ).FirstOrDefaultAsync();
        return result;
    }


    public async Task<List<OrderItemModifiersMapping>> GetAllOrderItemModifiersMapping(){
        return await _context.OrderItemModifiersMappings.ToListAsync();
    }
    public async Task<List<OrderItemMapping>> GetAllOrderItemMapping(){
        return await _context.OrderItemMappings.ToListAsync();
    }

    
    public async Task<OrderItemMapping?> GetOrderItemMappingById(int OrderItemMappingId){
        return await _context.OrderItemMappings.Where(x => x.Orderitemmappingid == OrderItemMappingId).FirstOrDefaultAsync();
    }

    public async Task<string> UpdateOrderItemMapping(OrderItemMapping order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> UpdateOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> AddOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    public async Task<string> DeleteOrderTaxMapping(OrderTaxMapping order)
    {
        try{
            _context.Remove(order);
            await _context.SaveChangesAsync();
            return "removed";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<string> AddOrderItemMapping(OrderItemMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }
    
    public async Task<string> AddOrderItemModifiersMapping(OrderItemModifiersMapping order)
    {
        try{
            _context.Add(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }


    public async Task<List<OrderItemMapping>?> GetOrderItemMappingByOrderId(int OrderId)
    {
        List<OrderItemMapping>? result = await (
                from oi in _context.OrderItemMappings
                where oi.Orderid == OrderId
                select new OrderItemMapping
                {
                    Orderitemmappingid = oi.Orderitemmappingid,
                    Itemid = oi.Itemid,
                    Orderid = oi.Orderid,
                    Totalquantity = oi.Totalquantity,
                    Readyquantity = oi.Readyquantity,
                    Status = oi.Status,
                    Specialmessage = oi.Specialmessage,
                    Createdat = oi.Createdat
                }
        ).ToListAsync();

        return result;
    }

    public async Task<string> UpdateOrder(Order order)
    {
        try{
            _context.Update(order);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid)
    {
        return await _context.OrderItemModifiersMappings.Where(o => o.Orderitemmappingid == Orderitemmappingid).ToListAsync();
    }

    public async Task RemoveRangeOfOrderItemModifiersMappings(List<OrderItemModifiersMapping> mappings)
    {
        _context.OrderItemModifiersMappings.RemoveRange(mappings);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderItemMapping(OrderItemMapping orderItemMapping)
    {
        _context.OrderItemMappings.Remove(orderItemMapping);
        await _context.SaveChangesAsync();
    }


    public async Task<List<OrderTaxMapping>?> GetOrderTaxMappingByOrderId(int OrderId)
    {
        List<OrderTaxMapping>? result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId
                select new OrderTaxMapping
                {
                    Ordertaxmappingid = ot.Ordertaxmappingid,
                    Taxid = ot.Taxid,
                    Orderid = ot.Orderid,
                    Totalamount = ot.Totalamount,
                    Createdat = ot.Createdat,
                    Editedat = ot.Editedat,
                    Createdbyid = ot.Createdbyid,
                }
        ).ToListAsync();

        return result;
    }





    
    // functions
    public async Task<List<OrderItemModifiersMappingViewModel>> FunctionGetOIMByOrderId(int orderid)
    {   
        try
        {
            var query = "select * from Fun_GetOIMByOrderId(@OrderId)";
            var parameters = new { OrderId = orderid };
            var result = await _dbConnection.QueryAsync<OrderItemModifiersMappingViewModel>(query,parameters);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

   
    // functions
    public async Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin()
    {
        try
        {
            var query = "select * from Fun_GetAllOrderItemModifierJoin()";
            var result = await _dbConnection.QueryAsync<OrderItemModifierJoinModelView>(query);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

    public async Task<List<WaitingListViewModel>> Function_GetAllWaitingList_OrderApp()
    {
        try
        {
            var query = "select * from Function_GetAllWaitingList_OrderApp()";
            var result = await _dbConnection.QueryAsync<WaitingListViewModel>(query);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }
    
    public async Task<List<WaitingListViewModel>> Function_GetAllWaitingList_By_SectionId_OrderApp(int? sectionid = null)
    {
        try
        {
            var query = "select * from Function_GetAllWaitingList_By_SectionId_OrderApp(@sectionid)";
            var parameters = new { sectionid = sectionid };
            var result = await _dbConnection.QueryAsync<WaitingListViewModel>(query, parameters);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }
    
    public async Task<int> Fun_customer_Add_Edit_byId(string Customername,string Customeremail,decimal Customerphone,int userid)
    {
        try{
            var query = "SELECT Fun_customer_Add_Edit_byId(@Customername, @Customeremail, @Customerphone, @userid)";
            var result = await _dbConnection.QuerySingleAsync<int>(query, new { Customername, Customeremail, Customerphone, userid });
            return result;
        }catch(Exception e){
            throw new Exception(e.Message);
        }
    }

    //sp
    public async Task Sp_Delete_waitinglist(int Waitingid,int userid)
    {
        try{
            var query = "CALL Sp_Delete_waitinglist(@Waitingid,@userid)";
            var parameters = new { Waitingid = Waitingid, userid = userid };
            await _dbConnection.ExecuteAsync(query, parameters);
        }catch(Exception e){
            throw new Exception(e.Message);
        }
    }

    public async Task PsUpdateOrderStatus(int orderId, bool status)
    {
        try{
            // await _context.Database.ExecuteSqlRawAsync("CALL sp_update_order_status({0}, {1})", orderId, status);
            var query = "CALL sp_update_order_status(@OrderId, @Status)";
            var parameters = new { OrderId = orderId, Status = status };
            await _dbConnection.ExecuteAsync(query, parameters);
        }
        catch(Exception e){
            throw new Exception(e.Message);
        }
    }

    public async Task Ps_UpdateOrderItemMapping_status_readyquantity(int OrderItemMappingId,int ReadyQuantity, int status)
    {
        try{
            // await _context.Database.ExecuteSqlRawAsync("CALL sp_UpdateOrderItemMapping_status_readyquantity({0}, {1}, {2})", OrderItemMappingId, ReadyQuantity, status);
            var query = "CALL sp_UpdateOrderItemMapping_status_readyquantity(@OrderItemMappingId, @ReadyQuantity, @Status)";
            var parameters = new { OrderItemMappingId = OrderItemMappingId, ReadyQuantity = ReadyQuantity, Status = status };
            await _dbConnection.ExecuteAsync(query, parameters);
        }
        catch(Exception e){
            throw new Exception(e.Message);
        }
    }


    public async Task<(int CustomerId, int OrderId)> CallSpAddCustomerAsync(
    string customerName, 
    string customerEmail, 
    decimal customerPhone, 
    int waitingId, 
    int totalPersons, 
    int totalAmount, 
    int userId)
    {
        try
        {
            var parameters = new DynamicParameters();

            // Match parameter order and names to your procedure
            parameters.Add("in_customername", customerName, DbType.String, ParameterDirection.Input);
            parameters.Add("in_customeremail", customerEmail, DbType.String, ParameterDirection.Input);
            parameters.Add("in_customerphone", customerPhone, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("in_waitingId", waitingId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_totoalpersons", totalPersons, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_totalamount", totalAmount, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_userid", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("out_customerid", 0, DbType.Int32, ParameterDirection.InputOutput);
            parameters.Add("out_orderid", 0, DbType.Int32, ParameterDirection.InputOutput);

            // Use CALL with CommandType.Text for PostgreSQL procedures
            string sql = "CALL Sp_Add_Customer(@in_customername, @in_customeremail, @in_customerphone, @in_waitingId, @in_totoalpersons, @in_totalamount, @in_userid, @out_customerid, @out_orderid);";

            await _dbConnection.ExecuteAsync(sql, parameters, commandType: CommandType.Text);

            int outCustomerId = parameters.Get<int>("out_customerid");
            int outOrderId = parameters.Get<int>("out_orderid");

            return (outCustomerId, outOrderId);
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

    public async Task sp_UpdateOrderItemMapping(OrderItemMapping orderItemMapping)
    {
        try
        {
            var query = "CALL sp_UpdateOrderItemMapping(@Orderitemmappingid, @Itemid, @Orderid, @Totalquantity, @Readyquantity, @Status, @Specialmessage)";
            var parameters = new
            {
                Orderitemmappingid = orderItemMapping.Orderitemmappingid,
                Itemid = orderItemMapping.Itemid,
                Orderid = orderItemMapping.Orderid,
                Totalquantity = orderItemMapping.Totalquantity,
                Readyquantity = orderItemMapping.Readyquantity,
                Status = orderItemMapping.Status,
                Specialmessage = orderItemMapping.Specialmessage
            };
            await _dbConnection.ExecuteAsync(query, parameters);
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

    public async Task sp_AddFeedback(Feedback feedback)
    {
        try
        {
            var query = "CALL sp_AddFeedback(@Orderid, @FoodRating, @AmbienceRating, @ServiceRating, @CommentsFeedback, @Createdat, @Createdbyid, @Editedbyid, @Editedat)";
            var parameters = new
            {
                Orderid = feedback.Orderid,
                FoodRating = feedback.FoodRating,
                AmbienceRating = feedback.AmbienceRating,
                ServiceRating = feedback.ServiceRating,
                CommentsFeedback = feedback.CommentsFeedback,
                Createdat = feedback.Createdat,
                Createdbyid = feedback.Createdbyid,
                Editedbyid = feedback.Editedbyid,
                Editedat = feedback.Editedat
            };
        
            await _dbConnection.ExecuteAsync(query, parameters);
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

    public async Task<bool> sp_EditWaitingListAsync(int waitingId, string cutomername, string customeremail,decimal customerphone, int totalPersons, int sectionid, int userId)
    {
        try
        {
            var query = "CALL Sp_EditWaitingList(@in_WaitingId, @in_Cutomername, @in_Customeremail, @in_Customerphone, @in_TotalPersons, @in_Sectionid, @in_UserId, @out_response)";
            var parameters = new DynamicParameters();
            parameters.Add("in_WaitingId", waitingId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_Cutomername", cutomername, DbType.String, ParameterDirection.Input);
            parameters.Add("in_Customeremail", customeremail, DbType.String, ParameterDirection.Input);
            parameters.Add("in_Customerphone", customerphone, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("in_TotalPersons", totalPersons, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_Sectionid", sectionid, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_UserId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("out_response", dbType: DbType.Boolean, direction: ParameterDirection.InputOutput);
           
            await _dbConnection.ExecuteAsync(query, parameters);
            bool response  = parameters.Get<bool>("out_response");
            

            return response;
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }

    public async Task<bool> Sp_Cancel_Complete_OrderHelper(int orderId, int OIstatus, int tableStatus, int orderStatus, int userId, decimal avgRating)
    {
        try
        {
            var query = "CALL Sp_Cancel_Complete_OrderHelper(@in_orderId, @in_OI_status, @in_table_status, @in_order_status, @in_userid, @in_Avg_rating, @out_response)";
            var parameters = new DynamicParameters();
            parameters.Add("in_orderId", orderId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_OI_status", OIstatus, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_table_status", tableStatus, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_order_status", orderStatus, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_userid", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_Avg_rating", avgRating, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("out_response", dbType: DbType.Boolean, direction: ParameterDirection.InputOutput);

            await _dbConnection.ExecuteAsync(query, parameters);
            bool response = parameters.Get<bool>("out_response");

            return response;
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }


    // sp - edit customer at order app
    public async Task<bool> Sp_EditCustomerDetails(
        int customerId, 
        string customerName, 
        string customerEmail, 
        decimal customerPhone,
        int orderId,
        int totalPersons, 
        int userId
    )
    {
        try
        {
            var query = "CALL Sp_EditCustomerDetails(@in_CustomerId, @in_CustomerName, @in_CustomerEmail, @in_CustomerPhone, @in_OrderId, @in_TotalPersons, @in_UserId, @out_Response)";
            var parameters = new DynamicParameters();
            parameters.Add("in_CustomerId", customerId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_CustomerName", customerName, DbType.String, ParameterDirection.Input);
            parameters.Add("in_CustomerEmail", customerEmail, DbType.String, ParameterDirection.Input);
            parameters.Add("in_CustomerPhone", customerPhone, DbType.Decimal, ParameterDirection.Input);
            parameters.Add("in_OrderId", orderId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_TotalPersons", totalPersons, DbType.Int32, ParameterDirection.Input);
            parameters.Add("in_UserId", userId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("out_Response", dbType: DbType.Boolean, direction: ParameterDirection.InputOutput);

            await _dbConnection.ExecuteAsync(query, parameters);
            bool response = parameters.Get<bool>("out_Response");

            return response;
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing stored procedure: {e.Message}", e);
        }
    }


    public async Task<Order?> Function_GetOrderById(int orderid)
    {
        try
        {
            var query = "SELECT * FROM Function_GetOrderById(@OrderId)";
            var parameters = new { OrderId = orderid };
            var result = await _dbConnection.QuerySingleOrDefaultAsync<Order>(query, parameters);
            return result;
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing function: {e.Message}", e);
        }
    }

    public async Task<List<OrderItemMapping>?> Function_GetOrderItemMappingByOrderId(int orderid)
    {
        try
        {
            var query = "SELECT * FROM Function_GetOrderItemMappingByOrderId(@OrderId)";
            var parameters = new { OrderId = orderid };
            var result = await _dbConnection.QueryAsync<OrderItemMapping>(query, parameters);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing function: {e.Message}", e);
        }
    }

    public async Task<List<OrderItemModifiersMappingViewModel>> Function_GetOIMByOrderId(int orderid)
    {
        try
        {
            var query = "SELECT * FROM Function_GetOIMByOrderId(@OrderId)";
            var parameters = new { OrderId = orderid };
            var result = await _dbConnection.QueryAsync<OrderItemModifiersMappingViewModel>(query, parameters);
            return result.ToList();
        }
        catch (Exception e)
        {
            throw new Exception($"Error executing function: {e.Message}", e);
        }
    }


}
