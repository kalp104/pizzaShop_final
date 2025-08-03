using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface ILoginService
{
    public Task<bool> GetLoginService(LoginViewModel model);
    public Task<Account?> GetAccoutAsync(string email);
    public Task<string?> ResetPasswordService(ForgetPasswordViewModel model);
    public Task<PasswordResetRequest?> ResetPasswordGetService(string token);
    public Task<bool> ForgetPasword(string email);
    public Task<Account?> GetAccountById(int accountId);
    Task<ResponseTokenViewModel> Validate2faToken(User2FAViewModel model);
}
