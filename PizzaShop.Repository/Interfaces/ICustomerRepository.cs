using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ICustomerRepository 
{   
    Task<Customer?> GetCutomerById(int customerid);
    Task<List<Customer>?> GetAllCategoryAsync();
    Task<List<OrdersCustomersMapping>?> GetOrdersCustomersMappingByCustomerId(int customerid);
    Task<List<OrdersCustomersMapping>?> GetAllOrdersCustomersMapping();
    Task<List<Customer>?> GetCustomersByFilterDates(DateTime startDate, DateTime endDate);
    Task<Customer?> CheckCustomerByEmail(string email);
    Task UpdateCustomer(Customer customer);
    Task AddCustomer(Customer customer);
    Task<List<Customer>> GetCustomerByEmail(string email);
}

