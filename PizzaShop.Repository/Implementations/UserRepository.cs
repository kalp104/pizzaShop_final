using System;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly PizzaShop2Context _context;
    private IDbConnection _dbConnection { get; }

    public UserRepository(PizzaShop2Context context, IDbConnection dbConnection)
    {
        _context = context;
        _dbConnection = dbConnection;       
    }

    public async Task<User?> GetUserByAccountId(int accountid)
    {
        return await _context.Users
                     .Where(x => x.Accountid == accountid)
                     .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserById(int userid)
    {
        return await _context.Users
                     .Where(x => x.Userid == userid)
                     .FirstOrDefaultAsync();
    }


    public async Task<List<Country>?> GetAllCountries()
    {
        return await _context.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountryByid(int countryid)
    {
        return await _context.Countries.Where(x=>x.Countryid ==countryid).FirstOrDefaultAsync();
    }

    public async Task<List<State>?> GetAllStates(int countryid)
    {
        return await _context.States
                     .Where(x => x.Countryid == countryid)
                     .ToListAsync();
    }

    public async Task<State?> GetStateByid(int stateid)
    {
        return await _context.States.Where(x=>x.Stateid ==stateid).FirstOrDefaultAsync();
    }

    public async Task<List<City>?> GetAllCities(int stateid)
    {
        return await _context.Cities
                     .Where(x => x.Stateid == stateid)
                     .ToListAsync();;
    }

    public async Task<City?> GetCityByid(int cityid)
    {
        return await _context.Cities.Where(x=>x.Cityid ==cityid).FirstOrDefaultAsync();
    }

    public async Task<string> UpdateUser(User user)
    {
         try{
            _context.Update(user);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        } 
        
    }

    public async Task<List<ItemWithCount>> GetTopItemsOrderedAsync(int range, DateTime startDate, DateTime endDate)
    {
            var result = await (
                from oi in _context.OrderItemMappings
                join o in _context.Orders on oi.Orderid equals o.Orderid
                join i in _context.Items on oi.Itemid equals i.Itemid
                where o.Createdat >= startDate && o.Createdat <= endDate
                group new { oi, i } by new { i.Itemname, i.Imageid } into g
                orderby g.Count() descending
                select new ItemWithCount
                {
                    ItemName = g.Key.Itemname,
                    Count = g.Count(),
                    Image = g.Key.Imageid
                }
            ).Take(range).ToListAsync();

            return result;
    }
    public async Task<List<ItemWithCount>> GetLastItemsOrderedAsync(int range, DateTime startDate, DateTime endDate)
    {
            var result = await (
                from oi in _context.OrderItemMappings
                join o in _context.Orders on oi.Orderid equals o.Orderid
                join i in _context.Items on oi.Itemid equals i.Itemid
                where o.Createdat >= startDate && o.Createdat <= endDate
                group new { oi, i } by new { i.Itemname, i.Imageid } into g
                orderby g.Count() ascending
                select new ItemWithCount
                {
                    ItemName = g.Key.Itemname,
                    Count = g.Count(),
                    Image = g.Key.Imageid
                }
            ).Take(range).ToListAsync();

            return result;
    }

    public async Task<List<GraphRevenueViewModel>> GetGraphDataAsync(DateTime startDate, DateTime endDate)
    {
            var result = await (
                from o in _context.Orders
                where o.Isdeleted == false && o.Createdat >= startDate && o.Createdat <= endDate && o.Status == 5
                group o by o.Createdat.Value.Date into g
                orderby g.Key ascending
                select new GraphRevenueViewModel
                {
                    revenue = g.Sum(x => x.Totalamount),
                    date = g.Key,
                    dateNumber = g.Key.ToString("MMM dd")
                }
            ).ToListAsync();

            return result;
    }

    public async Task<List<GraphCustomerViewModel>> GetCustomerGraphDataAsync(DateTime startDate, DateTime endDate)
    {
            var result = await (
                from o in _context.Customers
                where o.Isdeleted == false && o.Createdat >= startDate && o.Createdat <= endDate 
                group o by o.Createdat.Value.Date into g
                orderby g.Key ascending
                select new GraphCustomerViewModel
                {
                    NumberOfCustomer = g.Select(x => x.Customerid).Distinct().Count(),
                    Date = g.Key,
                    Month = g.Key.ToString("MMM dd")
                }
            ).ToListAsync();
            
            // var result = await (
            //     from o in _context.Customers
            //     join oc in _context.OrdersCustomersMappings on o.Customerid equals oc.Customerid
            //     where o.Isdeleted == false && oc.Createdat >= startDate && oc.Createdat <= endDate 
            //     group oc by oc.Createdat.Value.Date into g
            //     orderby g.Key ascending
            //     select new GraphCustomerViewModel
            //     {
            //         NumberOfCustomer = g.Select(x => x.Customerid).Distinct().Count(),
            //         Date = g.Key,
            //         Month = g.Key.ToString("MMM dd")
            //     }
            // ).ToListAsync();
            return result;
    }

    public async Task<List<UserTableViewModel>?> UserDetailAsync()
    {
        List<UserTableViewModel> list =  await (
                from u in _context.Users
                join a in _context.Accounts on u.Accountid equals a.Accountid
                join r in _context.Roles on u.Roleid equals r.Roleid
                where u.Isdeleted == false && a.Isdeleted == false
                orderby u.Userid descending
                select new UserTableViewModel
                {
                    AccountId = a.Accountid,
                    Id = u.Userid,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = a.Email,
                    Phone = u.Phone,
                    Role = a.Roleid,
                    Rolename = r.Rolename,
                    Status = u.Status,
                    Image = u.Userimage
                }
        ).ToListAsync();
        return list;
    }


    public async Task<string> AddUser(User user)
    {
        try{
            _context.Add(user);
            await _context.SaveChangesAsync();
            return "saved";
        }
        catch(Exception e)
        {
            return "";
        }   
    }

    public async Task<List<UserTableViewModel>?> Function_GetUsers_Pagging(
        int page = 1,
        int pageSize = 5,
        string searchTerm = "",
        string? sortBy = null, //  name,role
        string? sortDirection = null //  "asc", "desc"
    )
    {
        try{
            var query = "SELECT * FROM Function_GetUsers_Pagging( @searchTerm, @page, @pageSize, @sortBy, @sortDirection)";
            var parameters = new {searchTerm = searchTerm, page = page, pageSize = pageSize, sortBy = sortBy, sortDirection = sortDirection};
            var result = await _dbConnection.QueryAsync<UserTableViewModel>(query, parameters);
            return result.ToList();
        }
        catch(Exception e)
        {
            throw new Exception("Error fetching users: " + e.Message);
        }
    }

    public async Task<int> Function_GetUsersCount(string searchTerm = "")
    {
        try{
            var query = "SELECT * FROM Function_GetUsersCount(@searchTerm)";
            var parameters = new {searchTerm = searchTerm};
            var result = await _dbConnection.QuerySingleAsync<int>(query, parameters);
            return result;
        }
        catch(Exception e)
        {
            throw new Exception("Error fetching users count: " + e.Message);
        }
    }

}
