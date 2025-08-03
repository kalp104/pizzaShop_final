using System;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface IUserService
{
    public Task<UserBagViewModel?> UserDetailBag(string email);
    public Task<List<RolePermissionModelView>?> RoleFilter(string rolename);
    public Task<RolePermissionModelView?> PermissionFilter(string rolename, int permission);
    public Task<List<State>?> GetStates(int id);
    public Task<List<City>?> GetCities(int id);
    public Task<List<Country1>?> GetCountries();
    public Task<string?> UpdateUserProfile(UserViewModel model);
    public Task<string?> UpdateImage(int userId, IFormFile imageFile);
    public Task<string?> ChangePassword(ResetPasswordViewModel model, string email);

    public Task<DashBoardViewModel?> GetDashboardData(int? selector = null,DateTime? fromDate = null, DateTime? toDate = null);
}
