using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IUserTableService
{
    Task<List<UserTableViewModel>?> GetUsersDetails(
         int page = 1,
        int pageSize = 5,
        string searchTerm = "",
        string? sortBy = null, //  name,role
        string? sortDirection = null //  "asc", "desc"
    );
    Task<bool> DeleteUser(int id,int Userid);
    Task<UserViewModel> EditUserById(int id);
    Task<bool> EditUserPostAsync(UserViewModel model, [FromForm] IFormFile imageFile);
    Task<string?> AddUserService(AddNewUserViewModel model, [FromForm] IFormFile imageFile, int Userid);

    Task<bool> EmailSendToNewUser(string email, string password);
    Task<int> GetUsersCount(string searchTerm = "");

}
