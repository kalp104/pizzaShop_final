using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Interfaces;

public interface IGenericRepository<T>
    where T : class
{
    Task<Account?> GetAccountByEmail(string email);
    Task<Account?> GetAccountByUsername(string username);
    Task<string?> UpdateAsync(T model);
    Task<User?> GetUserByIdAsync(int id);
    Task<List<RolePermissionModelView>?> GetRolePermissionModelViewAsync(int roleid);
    Task<RolePermissionModelView?> GetPermissionAsync(int roleid, int permission);
    Task<List<UserTableViewModel>?> UserDetailAsync();
    Task<T?> GetByIdAsync(int? id);
    Task<List<T>> GetAllAsync();
    Task<List<City>> GetCityListByIdAsync(int id);
    Task<List<State>> GetStateListByIdAsync(int id);
    Task AddAsync(T model);
    Task<List<RolePermissionModelView>?> GetPermissionAsync(int id);
    Task<PermissionsRole?> GetRoleAndPermissionAsync(int roleid, int permissionid);
    Task<List<Category>> GetAllCategoryAsync();
    Task<List<Modifiergroup>> GetAllModifierGroupAsync();
    Task<List<Item>> GetAllItemsAsync();
    Task<List<Modifier>> GetAllModifierAsync();
    Task<List<Item>> GetItemsByCategoryAsync(int? id);
    Task<List<Modifier>> GetModifiersByMGAsync(int? id);
    Task<Modfierandgroupsmapping?> GetMappingsById(int Modifiergroupid, int modifierid);
    Task<List<Modfierandgroupsmapping>?> GetByModifierIdAsync(int? modifiersid);
    Task<List<Modfierandgroupsmapping>?> GetByModifierGroupIdAsync(int? modifierGroupId);
    Task DeleteAsync(T entity);
    Task<List<ItemModifiergroupMapping>?> GetByItemIdAsync(int? itemId);
    Task<List<ItemModifiergroupMapping>?> GetByModifierGroupIdMappingAsync(int? modifierGroupId);

    // section and table halpers
    Task<List<Table>> GetTableBySectionIdAsync(int? id);

    // orders
    Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync();
    Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId);
    Task<List<OrderItemMapping>> GetOrderItemMappingByOrderId(int OrderId);
    Task<List<OrderTaxMapping>> GetOrderTaxMappingByOrderId(int OrderId);

    // order app KOT
    Task<List<CustomersViewModel>> GetAllCustomersOrderDetails();
    Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid);
    Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin();

    // order app table

    Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid);
    Task<List<Customer>> GetCustomerByEmail(string email);

    Task<OrdersTablesMapping> GetOrderByTableId(int tableid);

    Task<List<TaxAmountViewModel>> GetTaxByOrderId(int OrderId);


    // duplication handler
    Task<List<TaxAndFee>?> CheckTaxByName(string name);
    Task<List<Section>?> CheckSectionByName(string name);
    Task<List<Table>?> CheckTableByName(string name);
    Task<List<Category>?> CheckCategoryByName(string name);
    Task<List<Modifiergroup>?> CheckModifierGroupByName(string name);
    Task<List<Item>?> CheckItemByName(string name);
    Task<List<Modifier>?> CheckModifierByName(string name);
    Task<Customer?> CheckCustomerByEmail(string email);
    Task<List<OrderTax>> GetTaxesByOrderId(int orderId);
    Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid);



    // Dashboard 
    Task<List<ItemWithCount>> GetTopItemsOrderedAsync(int range, DateTime startDate, DateTime endDate);
    Task<List<ItemWithCount>> GetLastItemsOrderedAsync(int range, DateTime startDate, DateTime endDate);
    Task<List<GraphRevenueViewModel>> GetGraphDataAsync(DateTime startDate, DateTime endDate);
    Task<List<GraphCustomerViewModel>> GetCustomerGraphDataAsync(DateTime startDate, DateTime endDate);

    // feedback
    Task<Feedback?> GetFeedbackByOrderId(int orderid);
    Task<PasswordResetRequest?> GetTokenData(string token);
}
