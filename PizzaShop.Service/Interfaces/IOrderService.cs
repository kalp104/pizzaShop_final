using System;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IOrderService
{
    Task<OrdersHelperModelView> GetOrders(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        int? status = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null,
        string? sortDirection = null
    );

    Task<OrderDetailsHelperViewModel?> getOrderDetails(int orderId);

    Task<byte[]> GenerateInvoicePdf(int orderId);
}
