using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IOrderAppService
{
    Task<OrderAppKOTViewModel> GetCardDetails(int? category = null, int? status = null, bool? IsModal = false);
    Task UpdateReadyQuantitiesAsync(int orderId, List<UpdateReadyQuantityModel> updates, int Status);
    Task<List<WaitingListViewModel>> GetAllWaitingList();
    Task<List<WaitingListViewModel>> GetAllWaitingListBySectionId(int? sectionId = null);
    Task<OrderAppMenuViewModel?> AddCustomer(OrderAppTableViewModel model, int userid);

    Task<OrderAppWaitingTokenViewModel> GetWaitingTokens(int? sectionId = null);

    Task<OrderAppWaitingTokenViewModel> GetCustomerDetails(string email);

    Task<bool> DeleteWaitingToken(int waitingId, int userid);
    Task<bool> EditWaitingToken(OrderAppTableViewModel model,int userid);

    Task<OrderAppMenuViewModel> GetOrderPageDetailByTableId(int tableId);

    Task<List<Table>?> GetTablesBySectionId(int? sectionId = null,int? capacity = null);

    Task<bool> EditCustomerDetails(CustomerEditViewModel model,int userid);

    Task<int> CompleteOrder(int orderid, int userid, FeedbackViewModel feedback);
    
    Task<bool> AddOrderDetails(OrderDataViewModel model);

    Task<bool> CancelOrder(int orderid, int userid);
    Task<bool> AddFavourite(int itemid);
    Task<OrderDataViewModel> GetOrderItemDetails(int OrderId);

    Task<bool> CheckStateOfItems(int orderid);

}
