using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<OrdersHelperModelView> GetCustomers(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 5,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? sortBy = null,
        string? sortDirection = null
    );

    Task<OrdersHelperModelView> getCustomerHistory(int customerId);

    Task<(byte[] FileContent, string FileName)> ExportCustomers(
        string? searchTerm = null,
        string? dateRange = null,
        string? fromDate = null,
        string? toDate = null,
        string? userRole = null
    );
}
