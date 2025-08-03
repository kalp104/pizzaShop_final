using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class LoginService : ILoginService
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILoginRepository _loginRepository;

    public LoginService(
        IEmailService emailService,
        IHostEnvironment environment,
        IConfiguration configuration,
        ILoginRepository loginRepository  
    )
    {
        _configuration = configuration;
        _emailService = emailService;
        _environment = environment;
        _loginRepository = loginRepository;        
    }

    enum Roles
    {
        AccountManager = 1,
        Chef = 2,
        Admin = 3,
    }

    public async Task<bool> GetLoginService(LoginViewModel model)
    {
       try{
            Account? account = await _loginRepository.GetAccountByEmail(model.Email.Trim());

            if (account != null)
            {
                string? rolename = account.Roleid != 0 && account.Roleid.HasValue ? ((Roles)account.Roleid.Value).ToString() : null;
                if (
                    rolename != null
                    && BCrypt.Net.BCrypt.EnhancedVerify(model.Password, account.Password)
                )
                {

                    Random random = new Random();
                    int randomNumber = random.Next(100001, 1000000); // Generate a random number between 100000 and 999999
                    string randomString = randomNumber.ToString();
                    string varificationCode = BCrypt.Net.BCrypt.EnhancedHashPassword(randomString);

                    User2faAuth user2faAuth = new User2faAuth
                    {
                        TokenAuth = varificationCode,
                        Email = model.Email,
                        Rememberme = model.Rememberme,
                        CreateTime = DateTime.Now,
                        ExpireTime = DateTime.Now.AddMinutes(5)
                    };

                    await _loginRepository.AddUser2faAuth(user2faAuth);

                    // Generate 2FA code email to user
                
                    string emailBody =
                        $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Email Template</title>
                        </head>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px;'>
                            <header>
                                <h1 style='color:#fff;height:5rem;background-color:#0565a1;width:100%;display:flex;align-items:center;justify-content:center;'>PIZZASHOP</h1>
                            </header>
                            <p>Pizza shop,</p>
                            <p>Here is you varification code : </p>
                            <h2 style='color:#0565a1;width:100%;display:flex;align-items:center;justify-content:center;'>{randomString}</h2>
                            <p>If you encounter any issue or have any question, please do not hesitate to contact our support team.</p>
                            <p><span style='color:#8B8000'>Important note:</span> For security purposes, This will expire in 5 minutes. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
                        </body>
                        </html>";

                    await _emailService.SendEmailAsync(
                        model.Email,
                        "token for authentication",
                        emailBody
                    );

                    return true;

                    // if (model?.Rememberme == null)
                    // {
                    //     model.Rememberme = false;
                    // }
                    // var TokenExpireTime = model.Rememberme
                    //     ? DateTime.Now.AddDays(30)
                    //     : DateTime.Now.AddDays(1);
                    // var token = GenerateJwtToken(model.Email, TokenExpireTime, rolename);

                    // if (token != null)
                    // {
                        // return new ResponseTokenViewModel()
                        // {
                        //     token = token,
                        //     response = "Login successful",
                        // };
                    // }
                }
                else
                {
                    // return new ResponseTokenViewModel()
                    // {
                    //     token = "",
                    //     response = "Invalid User Credentials",
                    // };

                    return false; // invalid
                }
            }
            // return new ResponseTokenViewModel() { token = "", response = "Invalid User Credentials" };
            return false; // invalid
       }
       catch(Exception ex)
       {
            throw new Exception($"Error in LoginService: {ex.Message}");
       }

    }

    public async Task<ResponseTokenViewModel> Validate2faToken(User2FAViewModel model)
    {
        try{
            User2faAuth? user2faAuth = await _loginRepository.GetUser2faAuth(model.ToEmail.Trim());
            Account? account = await _loginRepository.GetAccountByEmail(model.ToEmail.Trim());
            if (user2faAuth != null)
            {
                if (user2faAuth.ExpireTime < DateTime.Now || user2faAuth.Counting >=3)
                {
                    // deleting all tokens which are generated for this email
                    await _loginRepository.deleteAllUser2faAuthByEmail(model.ToEmail.Trim());
                    return new ResponseTokenViewModel()
                    {
                        token = "",
                        response = "Token expired",
                    };
                }
                else
                {
                    if (BCrypt.Net.BCrypt.EnhancedVerify(model.Token, user2faAuth.TokenAuth))
                    {
                        string? rolename = account.Roleid != 0 && account.Roleid.HasValue ? ((Roles)account.Roleid.Value).ToString() : null;
                        var TokenExpireTime = user2faAuth.Rememberme
                            ? DateTime.Now.AddDays(30)
                            : DateTime.Now.AddDays(1);
                        var token = GenerateJwtToken(model.ToEmail, TokenExpireTime, rolename ?? "");
                        
                        // deleting all tokens which are generated for this email
                        await _loginRepository.deleteAllUser2faAuthByEmail(model.ToEmail.Trim());
                        
                        if (token != null)
                        {
                            return new ResponseTokenViewModel()
                            {
                                token = token,
                                response = "Login successful",
                            };
                        }
                    }
                    else
                    {
                        // update the count of the token 
                        // only 3 tries are allowed
                        user2faAuth.Counting = user2faAuth.Counting + 1;
                        await _loginRepository.UpdateUser2faAuth(user2faAuth);
                        return new ResponseTokenViewModel()
                        {
                            token = "",
                            response = $"Invalid User Credentials! only {3-user2faAuth.Counting} are left",
                        };
                    }
                }
            }
            return new ResponseTokenViewModel() { token = "", response = "Token expired" };

        }catch (Exception ex)
        {
            throw new Exception($"Error in LoginService: {ex.Message}");

        }
    }
    
    private string GenerateJwtToken(string email, DateTime expiryTime, string roleName)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //  Get user role from database
        // Console.WriteLine(roleName);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, roleName), // Add roles as needed
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiryTime,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<Account?> GetAccoutAsync(string email)
    {
        
        Account? account = await _loginRepository.GetAccountByEmail(email);
        return account;
    }

    public async Task<Account?> GetAccountById(int accountId)
    {
        
        Account? account = await _loginRepository.GetAccountById(accountId);
        return account;
    }

    public async Task<string?> ResetPasswordService(ForgetPasswordViewModel model)
    {
        if (model.Email != null)
        {
            
            Account? account = await _loginRepository.GetAccountByEmail(model.Email);
            if (account == null)
            {
                return "1"; // account not exists
            }
            if (model.Password != model.ConfirmPassword)
            {
                return "2"; // password doesnot match
            }
            if (
                account != null
                && BCrypt.Net.BCrypt.EnhancedVerify(model.Password, account.Password)
            )
            {
                return "3"; // password can not be same as previous one
            }

            if (account != null)
            {
                account.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(model.Password);
                string res = await _loginRepository.UpdateAccount(account);

                if (res == "saved")
                {
                    PasswordResetRequest? resetRequest = await _loginRepository.GetTokenData(model.Token);
                    if (resetRequest != null)
                    {
                        resetRequest.Closedate = DateTime.Now;
                        await _loginRepository.UpdatePasswordResetRequest(resetRequest);
                    }
                    return "4"; // successfully changed password
                }
            }
        }

        return ""; // invalid
    }



    public async Task<bool> ForgetPasword(string email)
    {
        if (email != null)
        {
            try
            {
                Account? account = await _loginRepository.GetAccountByEmail(email);
                if (account == null)
                {
                    return false; 
                }

                PasswordResetRequest resetRequest = new PasswordResetRequest
                {
                    Id = Guid.NewGuid(),
                    Userid = account.Accountid,
                    Createdate = DateTime.Now,
                    Guidtoken = Guid.NewGuid().ToString()
                };

                await _loginRepository.AddPasswordResetRequest(resetRequest);

                // Generate reset link with dynamic localhost URL in development
                string baseUrl = _environment.IsDevelopment()
                    ? "http://localhost:5095" // Adjust port if different
                    : _configuration["AppSettings:BaseUrl"] ?? "";
                
                string passwordResetLink = $"{baseUrl}/Home/ResetPassword?token={resetRequest.Guidtoken}";

                string resetLink = HtmlEncoder.Default.Encode(passwordResetLink);
                string emailBody =
                    $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Email Template</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f9; padding: 20px;'>
                        <header>
                            <h1 style='color:#fff;height:5rem;background-color:#0565a1;width:100%;display:flex;align-items:center;justify-content:center;'>PIZZASHOP</h1>
                        </header>
                        <p>Pizza shop,</p>
                        <p>Please click <a href='{resetLink}'>here</a> to reset your password.</p>
                        <p>If you encounter any issue or have any question, please do not hesitate to contact our support team.</p>
                        <p><span style='color:#8B8000'>Important note:</span> For security purposes, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately.</p>
                    </body>
                    </html>";

                await _emailService.SendEmailAsync(
                    email,
                    "Password Reset Request",
                    emailBody
                );

                return true;
            }
            catch (Exception ex)  
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return false; // invalid
            }
        }
        return false; // invalid
    }




    public async Task<PasswordResetRequest?> ResetPasswordGetService(string token){
        try
        {
            PasswordResetRequest? resetRequest = await _loginRepository.GetTokenData(token);
            if (resetRequest == null)
            {
                return null; // invalid token
            }
            return resetRequest;
                
        }catch(Exception ex){
            Console.WriteLine($"Failed to send email: {ex.Message}");
            return null;
        }
    }
}
