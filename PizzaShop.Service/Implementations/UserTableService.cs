using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class UserTableService : IUserTableService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;

    public UserTableService(
        IWebHostEnvironment webHostEnvironment,
        IEmailService emailService,
        IUserRepository userRepository,
        ILoginRepository loginRepository,
        IRoleRepository roleRepository
    )
    {
        _loginRepository = loginRepository;
        _userRepository = userRepository;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
        _roleRepository = roleRepository;

    }

    public async Task<int> GetUsersCount(string searchTerm = "")
    {
        return await _userRepository.Function_GetUsersCount(searchTerm);
    }
    public async Task<List<UserTableViewModel>?> GetUsersDetails(
        int page = 1,
        int pageSize = 5,
        string searchTerm = "",
        string? sortBy = null, //  name,role
        string? sortDirection = null //  "asc", "desc"
    )
    {
        
        List<UserTableViewModel>? result = await _userRepository.Function_GetUsers_Pagging(page, pageSize,searchTerm,sortBy,sortDirection);
        // List<UserTableViewModel>? result = await _userRepository.UserDetailAsync();
        // if (!string.IsNullOrEmpty(searchTerm))
        // {
        //     if (!string.IsNullOrEmpty(searchTerm))
        //     {
        //         searchTerm = searchTerm.ToLower();

        //         result = result
        //             .Where(u =>
        //                 u.Firstname.ToLower().Contains(searchTerm)
        //                 || u.Lastname.ToLower().Contains(searchTerm)
        //                 || u.Email.ToLower().Contains(searchTerm)
        //                 || u.Phone.ToString().Contains(searchTerm)
        //                 || (searchTerm.ToLower() == "admin" && u.Role == 3)
        //                 || (searchTerm.ToLower() == "chef" && u.Role == 2)
        //                 || (searchTerm.ToLower() == "accountmanager" && u.Role == 1)
        //                 || (searchTerm.ToLower() == "active" && u.Status == 1)
        //                 || (searchTerm.ToLower() == "inactive" && u.Status == 2)
        //                 || (searchTerm.ToLower() == "pending" && u.Status == 3)
        //             )
        //             .ToList();
        //     }

        // }

        //     if (!string.IsNullOrEmpty(sortBy))
        //     {
        //         switch (sortBy.ToLower())
        //         {
        //             case "name":
        //                 result =
        //                     sortDirection?.ToLower() == "asc"
        //                         ? result.OrderBy(o => o.Firstname).ToList()
        //                         : result.OrderByDescending(o => o.Firstname).ToList();
        //                 break;
        //             case "role":
        //                 result =
        //                     sortDirection?.ToLower() == "asc"
        //                         ? result.OrderBy(o => o.Rolename).ToList()
        //                         : result.OrderByDescending(o => o.Rolename).ToList();
        //                 break;
        //             default:
        //                 // Default sorting (e.g., by date descending)
        //                 result = result.OrderByDescending(o => o.AccountId).ToList();
        //                 break;
        //         }
        //     }
        //     else
        //     {
        //         // Default sorting if no sortBy specified
        //         result = result.OrderByDescending(o => o.AccountId).ToList();
        //     }       
        return result;
    }

    public async Task<bool> DeleteUser(int id,int Userid)
    {
        try
        {
            User? user = await _userRepository.GetUserById(id);
            Account? account = user != null ? await _loginRepository.GetAccountById(user.Accountid) : new Account();

            if (user != null && account != null)
            {
                user.Isdeleted = true;
                account.Isdeleted = true;
                user.Deletedat = DateTime.Now;
                account.Deletedat = DateTime.Now;
                user.Deletedbyid = Userid;
                account.Deletedbyid = Userid;
                await _userRepository.UpdateUser(user);
                await _loginRepository.UpdateAccount(account);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine("error while deleting user" + e.Message);
            return false;
        }
       
    }

    public async Task<UserViewModel> EditUserById(int id)
    {
        User? user = await _userRepository.GetUserById(id);
        Account? account = (user != null) ? await _loginRepository.GetAccountById(user.Accountid) : null;
        Role? role = (account != null && account.Roleid.HasValue) ? await _roleRepository.GetRoleById(account.Roleid.Value) : null;
        Country? country = (user != null) ? await _userRepository.GetCountryByid(user.Countryid) : null;
        State? state = (user != null) ? await _userRepository.GetStateByid(user.Stateid) : null;
        City? city = (user != null) ? await _userRepository.GetCityByid(user.Cityid) : null;
        List<Country>? countries = await _userRepository.GetAllCountries();
        List<Country1> country1List = countries
            .Select(c => new Country1 { CountryId = c.Countryid, CountryName = c.Countryname })
            .ToList();

        UserViewModel Users = new UserViewModel()
        {
            ImageUrl = user?.Userimage,
            Firstname = user?.Firstname,
            Lastname = user?.Lastname,
            Username = account?.Username,
            Email = account?.Email,
            Status = user?.Status,
            Countryname = country?.Countryname,
            Statename = state?.Statename,
            cityname = city?.Cityname,
            Zipcode = user?.Zipcode,
            Address = user?.Address,
            phone = user?.Phone,
            cityId = user?.Cityid ?? 0,
            stateId = user.Stateid,
            countryId = user.Countryid,
            userId = user.Userid,
            Countries = country1List,
            accountId = account?.Accountid ?? 0,
            roleId = user.Roleid,
            Rolename = role?.Rolename,
        };
        return Users;
    }

    public async Task<bool> EditUserPostAsync(UserViewModel model, [FromForm] IFormFile imageFile)
    {
        Account? account1 = model.Username != null ? await _loginRepository.GetAccountByUsername(model.Username) : null;
        if (account1 != null)
        {
            if (account1.Accountid != model.accountId )
            {
                return false;
            }
        }

        User? user = await _userRepository.GetUserById(model.userId);
        Account? account = (user != null) ? await _loginRepository.GetAccountById(user.Accountid) : null;
        try
        {
            if (imageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                if (user != null)
                {
                    user.Userimage = "/uploads/" + uniqueFileName;
                }
            }

            if (user != null && account != null)
            {
                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.Phone = model.phone;
                user.Zipcode = model.Zipcode;
                user.Address = model.Address;
                user.Cityid = model.cityId;
                user.Stateid = model.stateId;
                user.Countryid = model.countryId;
                user.Status = model.Status;
                user.Roleid = model.roleId;
                account.Roleid = model.roleId;
                account.Username = model.Username;
                account.Editedat = DateTime.Now;
                user.Editedat = DateTime.Now;

                await _loginRepository.UpdateAccount(account);
                await _userRepository.UpdateUser(user);
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("error while editing user" + e.Message);
            throw new Exception("Error while editing user: " + e.Message);
        }
    }

    public async Task<string?> AddUserService(
        AddNewUserViewModel model,
        [FromForm] IFormFile imageFile,
        int Userid
    )
    {
        if (model != null)
        {
            Account? account = model.Email != null ? await _loginRepository.GetAccountByEmail(model.Email) : null;

            User? user = account != null ? await _userRepository.GetUserByAccountId(account.Accountid) : null;
            if (account != null)
            {
                return "email already exists";
            }

            Account? account1 = model.Username != null ? await _loginRepository.GetAccountByUsername(model.Username) : null;
            if (account1 != null)
            {
                return "username already exists";
            }

            Account? newAccount = new()
            {
                Username = model.Username,
                Email = model.Email ?? throw new ArgumentNullException(nameof(model.Email)),
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password),
                Roleid = model.roleId,
                Isdeleted = false,
                Rememberme = false,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
                Createdbyid = Userid
            };
            await _loginRepository.AddAccount(newAccount);

            User newUser = new()
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Phone = model.phone,
                Address = model.Address,
                Zipcode = model.Zipcode,
                Countryid = model.countryId,
                Stateid = model.stateId,
                Cityid = model.cityId,
                Accountid = newAccount.Accountid,
                Roleid = model.roleId,
                Status = 2,
                Isdeleted = false,
                Createdat = DateTime.Now,
                Editedat = DateTime.Now,
                Createdbyid = Userid
            };

            if (imageFile != null)
            {
                string Imageurl = "/uploads/" + Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, Imageurl.Substring(9));

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }

                newUser.Userimage = Imageurl;
                await _userRepository.AddUser(newUser);
            }
        }

        return "";
    }



    public async Task<bool> EmailSendToNewUser(string email, string password)
    {
        try
        {
            string emailBody = 
            $@"
                <html> 
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Template</title>
                    </head>
                    <body style='width:100%; font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px;'>
                        <header style='width:100%;'>
                            <h1 style='color:#fff;height:5rem;background-color:#0565a1;width:100%;display:flex;align-items:center;justify-content:center;'>PIZZASHOP</h1>
                        </header>
                        <p>Welcome to Pizza shop</p>
                        <p>Please find the details below for login into your account</p>
                        <div style='width:80%; padding:10px; border:1px solid #000000;'>
                            <h5>Login Details: </h5>
                            <p style='font-weight:bolder'>Username : {email}</p>
                            <p style='font-weight:bolder'>Temporary Password : {password}</p>
                        </div>
                        <p>If you encounter any issue or have any question, please do not hesitate to contact our support team.</p>
                    </body>
                </html>
            ";
            await _emailService.SendEmailAsync(email, "New User", emailBody);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to send email to {email}: {e.Message}, InnerException: {e.InnerException?.Message}");
            return false;
        }
    }
}
