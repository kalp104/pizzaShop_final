using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly PizzaShop2Context _context;

    public CustomerRepository(PizzaShop2Context context)
    {
        _context = context;       
    }


    public async Task<Customer?> GetCutomerById(int customerid)
    {
        return await _context.Customers
                     .Where(x => x.Customerid == customerid)
                     .FirstOrDefaultAsync();
    }

    public async Task<List<Customer>?> GetAllCategoryAsync()
    {
        return await _context.Customers
                    .Where(x => x.Isdeleted == false )
                    .OrderByDescending(x => x.Customerid)
                    .ToListAsync();
    } 

    public async Task<List<OrdersCustomersMapping>?> GetOrdersCustomersMappingByCustomerId(int customerid)
    {
        return await _context.OrdersCustomersMappings
                     .Where(x => x.Customerid == customerid)
                     .ToListAsync();
    }

    public async Task<List<OrdersCustomersMapping>?> GetAllOrdersCustomersMapping()
    {
        return await _context.OrdersCustomersMappings.ToListAsync();
    }

    public async Task<List<Customer>?> GetCustomersByFilterDates(DateTime startDate, DateTime endDate)
    {
        return await _context.Customers 
                     .Where(c => c.Createdat >= startDate && c.Createdat <= endDate)
                     .ToListAsync();
    }

    public async Task<Customer?> CheckCustomerByEmail(string email)
    {
        return await _context.Customers.Where(s => s.Customeremail != null && s.Customeremail.Trim().ToLower().Equals(email) && s.Isdeleted == false).FirstOrDefaultAsync();
    }

    public async Task UpdateCustomer(Customer customer)
    {   
        _context.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task AddCustomer(Customer customer)
    {   
        _context.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Customer>> GetCustomerByEmail(string email)
    {
        return await _context
                .Customers.Where(c =>
                    c.Customeremail.ToLower().Contains(email) && c.Isdeleted == false
                )
                .ToListAsync();
    }
}
