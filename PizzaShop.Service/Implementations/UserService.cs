using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Repository.Helpers.Enums;

namespace PizzaShop.Service.Implementations;

public class UserService : IUserService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUserRepository _userRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly ICustomerRepository _customerRepository;

    public UserService(
        IWebHostEnvironment webHostEnvironment,
        IUserRepository userRepository,
        ILoginRepository loginRepository,
        IRoleRepository roleRepository,
        IOrderRepository orderRepository,
        IWaitingListRepository waitingListRepository,
        ICustomerRepository customerRepository
    )
    {
        _webHostEnvironment = webHostEnvironment;
        _userRepository = userRepository;
        _loginRepository = loginRepository;
        _roleRepository = roleRepository;
        _orderRepository = orderRepository;
        _waitingListRepository = waitingListRepository;
        _customerRepository = customerRepository;
    }

    public async Task<UserBagViewModel?> UserDetailBag(string email)
    {
        if (email != null)
        {
            Account? account = await _loginRepository.GetAccountByEmail(email);

            User? user = (account == null) ? null : await _userRepository.GetUserByAccountId(account.Accountid);

            if (account != null && user != null)
            {
                return new UserBagViewModel
                {
                    UserName = account.Username,
                    UserId = user.Userid,
                    ImageUrl = user.Userimage
                };
            }
        }
        return null;
    }

    public async Task<List<RolePermissionModelView>?> RoleFilter(string rolename)
    {
        int roleid = 0;
        switch (rolename)
        {
            case "Admin":
                roleid = 3;
                break;
            case "AccountManager":
                roleid = 1;
                break;
            case "Chef":
                roleid = 2;
                break;
            default:
                return null;
        }
        ;
        List<RolePermissionModelView>? result = await _roleRepository.GetRolePermissionModelViewAsync(roleid);
        if (result != null)
        {
            return result;
        }
        return null;
    }

    public async Task<RolePermissionModelView?> PermissionFilter(string rolename, int permission)
    {
        int roleid = 0;
        switch (rolename)
        {
            case "Admin":
                roleid = 3;
                break;
            case "AccountManager":
                roleid = 2;
                break;
            case "Chef":
                roleid = 1;
                break;
            default:
                return null;
        }
        ;
        // repo call : permission = 1 for users bar
        RolePermissionModelView? result = await _roleRepository.GetPermissionAsync(roleid, permission);

        if (result != null)
        {
            return result;
        }
        return null;
    }

    public async Task<List<Country1>?> GetCountries()
    {
        List<Country>? countries = await _userRepository.GetAllCountries();
        List<Country1> country1List = countries
            .Select(c => new Country1 { CountryId = c.Countryid, CountryName = c.Countryname })
            .ToList() ?? new List<Country1>();
        return country1List;
    }

    public async Task<List<State>?> GetStates(int id)
    {
        List<State>? state = await _userRepository.GetAllStates(id);
        if (state != null)
            return state;
        return null;
    }

    public async Task<List<City>?> GetCities(int id)
    {
        List<City>? city = await _userRepository.GetAllCities(id);
        if (city != null)
            return city;
        return null;
    }

    public async Task<string?> UpdateUserProfile(UserViewModel model)
    {
        try{

            Account? account1 = model.Username != null ? await _loginRepository.GetAccountByUsername(model.Username) : null;
            if (account1 != null)
            {
                if (account1.Accountid != model.accountId )
                {
                    return "Username already exists!";
                }
            }


            User? user = await _userRepository.GetUserById(model.userId);
            Account? account = (model != null) ? await _loginRepository.GetAccountById(model.accountId) : null;

            if (user == null && account == null)
            {
                return "profile update failed";
            }
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            account.Username = model.Username;
            user.Phone = model.phone;
            user.Address = model.Address;
            user.Zipcode = model.Zipcode;
            user.Countryid = model.countryId;
            user.Stateid = model.stateId;
            user.Cityid = model.cityId;
            user.Editedat = DateTime.Now;
            account.Editedat = DateTime.Now;

            string s = await _userRepository.UpdateUser(user);
            await _loginRepository.UpdateAccount(account);

            return "";
        }catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return "";
        }
    }

    public async Task<string?> UpdateImage(int userId, IFormFile imageFile)
    {
        User? user = await _userRepository.GetUserById(userId);
        if (user != null)
        {
            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
                
                    user.Userimage = "/uploads/" + uniqueFileName;
                    await _userRepository.UpdateUser(user);
                    return user.Userimage;
                
            }
            catch (Exception e)
            {
                System.Console.WriteLine("image uplode failed : " + e.Message);
            }
        }
        return "";
    }

    public async Task<string?> ChangePassword(ResetPasswordViewModel model, string email)
    {
        Account? account = await _loginRepository.GetAccountByEmail(email);
        if (account == null)
            return "1"; //"user doesnot exist"
        if (!BCrypt.Net.BCrypt.EnhancedVerify(model.CurrentPassword, account.Password))
            return "2"; //"password does not match"
        if (model.Password == model.CurrentPassword)
            return "3"; // New password cannot be the same as the old one.
        if (model.Password != model.ConfirmPassword)
            return "4"; // Passwords do not match.

        account.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);
        account.Editedat = DateTime.Now;
        await _loginRepository.UpdateAccount(account);
        return "0";
    }



    #region Dashboard
    public async Task<DashBoardViewModel?> GetDashboardData(int? selector = null,DateTime? fromDate = null, DateTime? toDate = null)
    {
        try
        {
            // Determine the time range based on selector
            DateTime endDate = DateTime.Now;
            DateTime startDate;
            bool isMonthly = false;

            switch (selector)
            {
                case 1: // Current month
                    startDate = new DateTime(endDate.Year, endDate.Month, 1);
                    isMonthly = true;
                    break;
                case 2: // Last 30 days
                    startDate = endDate.AddDays(-30);
                    break;
                case 3: // Last 7 days
                    startDate = endDate.AddDays(-7);
                    break;
                case 4: // today
                    startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day); // Start of today
                    endDate = startDate.AddDays(1).AddTicks(-1); // End of today (23:59:59.9999999)
                    break;
                    
                case 5: // Custom date 
                    if(fromDate != null && toDate != null)
                    {
                        if(fromDate.Value == toDate.Value)
                        {
                            startDate = new DateTime(endDate.Year, endDate.Month, endDate.Day);
                            endDate = startDate.AddDays(1).AddTicks(-1);
                        }else{
                            startDate = fromDate.Value;
                            endDate = toDate.Value.AddDays(1).AddTicks(-1);
                        }
                        break;
                    }
                    else{
                        startDate = new DateTime(endDate.Year, endDate.Month, 1);
                        isMonthly = true;
                        break;
                    }
                    
                default: // Default to current month
                    startDate = new DateTime(endDate.Year, endDate.Month, 1);
                    isMonthly = true;
                    break;
            }

            // Fetch data within the time range
            List<Order>? orders = await _orderRepository.GetOrderByFilterDates(startDate, endDate);

            List<Order>? OrderLists = await _orderRepository.GetOrderByFilterDates(startDate, endDate);

            List<WaitingList>? waitingLists = await _waitingListRepository.GetWaitingListsByDateFilters(startDate, endDate);

            List<ItemWithCount> topItems = await _userRepository.GetTopItemsOrderedAsync(2, startDate, endDate);

            List<ItemWithCount> lastItems = await _userRepository.GetLastItemsOrderedAsync(2, startDate, endDate);

            List<Customer>? customers = await  _customerRepository.GetCustomersByFilterDates(startDate, endDate);
           

            // Revenue graph data
            List<GraphRevenueViewModel>? graphDataForRevenue = await _userRepository.GetGraphDataAsync(startDate, endDate);
            List<GraphRevenueViewModel> graphDataRevenue = new List<GraphRevenueViewModel>();

            if (graphDataForRevenue != null)
            {
                if (isMonthly)
                {
                    int currentMonth = endDate.Month;
                    int currentDate = endDate.Day;
                    for (int i = 1; i <= currentDate; i++)
                    {
                        decimal revenue = graphDataForRevenue.Where(x => x.date.Day == i).Sum(x => Math.Round(x.revenue,2));
                        graphDataRevenue.Add(new GraphRevenueViewModel
                        {
                            revenue = revenue,
                            date = new DateTime(endDate.Year, currentMonth, i),
                            dateNumber = i.ToString()
                        });
                    }
                }
                else
                {
                    // For last 30 or 7 days, generate daily data points
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        decimal revenue = graphDataForRevenue.Where(x => x.date.Date == date.Date).Sum(x => x.revenue);
                        graphDataRevenue.Add(new GraphRevenueViewModel
                        {
                            revenue = revenue,
                            date = date,
                            dateNumber = date.ToString("MMM dd")
                        });
                    }
                }
            }

            // Customer graph data
            List<GraphCustomerViewModel>? graphDataForCustomer = await _userRepository.GetCustomerGraphDataAsync(startDate, endDate);
            List<GraphCustomerViewModel> graphDataCustomer = new List<GraphCustomerViewModel>();

            if (graphDataForCustomer != null)
            {
                if (isMonthly)
                {
                    // Monthly data for the current year
                    int currentMonth = endDate.Month;
                    int currentDate = endDate.Day;
                    for (int i = 1; i <= currentDate; i++)
                    {
                        int customerCount = graphDataForCustomer.Where(x => x.Date.Day == i).Sum(x => x.NumberOfCustomer);
                        graphDataCustomer.Add(new GraphCustomerViewModel
                        {
                            NumberOfCustomer = customerCount,
                            Month = i.ToString()
                        });
                    }
                }
                else
                {
                    // Daily data for last 30 or 7 days
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        int customerCount = graphDataForCustomer.Where(x => x.Date.Date == date.Date).Sum(x => x.NumberOfCustomer);
                        graphDataCustomer.Add(new GraphCustomerViewModel
                        {
                            NumberOfCustomer = customerCount,
                            Date = date,
                            Month = date.ToString("MMM dd")
                        });
                    }
                }
            }

            if (orders == null || OrderLists == null || topItems == null || lastItems == null || customers == null)
            {
                return null;
            }

            // Calculate metrics
            int totalOrders = orders.Count;
            decimal totalSales = orders.Where(x => x.Status == 5).Sum(x => x.Totalamount);
            decimal avgOrderValue = totalOrders == 0 ? 0 : Math.Round(totalSales / totalOrders, 2);

            List<DifferenceWaitingTime> datesForDifference = OrderLists.Select(x => new DifferenceWaitingTime
            {
                StartTime = x.Createdat,
                EndTime = x.Completedat
            }).ToList();

            List<TimeSpan> differences = datesForDifference
                .Select(d => d.StartTime.HasValue ? (d.EndTime ?? DateTime.Now) - d.StartTime.Value : TimeSpan.Zero)
                .ToList();

            TimeSpan averageDifference = differences.Any()
                ? TimeSpan.FromTicks((long)differences.Average(ts => ts.Ticks))
                : TimeSpan.Zero;

            int totalWaitingList = waitingLists.Count;
            string dateFormat = (averageDifference.Days > 0 ? $"{averageDifference.Days} Days " : "")
                            + (averageDifference.Hours > 0 ? $"{averageDifference.Hours} Hours " : "")
                            + (averageDifference.Minutes > 0 ? $"{averageDifference.Minutes} Min " : "")
                            + (averageDifference.Seconds > 0 ? $"{averageDifference.Seconds} Sec " : "");

            // Filter new customers (last 30 days or within selected range)
            customers = customers.Where(x => x.Isdeleted == false && x.Createdat >= startDate).ToList();

            return new DashBoardViewModel
            {
                TotalOrders = totalOrders,
                TotalSales = Math.Round(totalSales, 2),
                AvgOrderValue = avgOrderValue,
                TotalWaittingList = totalWaitingList,
                AvgWaitingTime = dateFormat.Trim(),
                topItems = topItems,
                LastItems = lastItems,
                TotalNewCustomer = customers.Count,
                graphDataRevenue = graphDataRevenue,
                GraphDataCustomer = graphDataCustomer
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching dashboard data: " + ex.Message);
            return null;
        }
    }

#endregion
}
