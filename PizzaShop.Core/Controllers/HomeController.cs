using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Models;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Core.Controllers;

public class HomeController : Controller
{
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILoginService _loginService;

    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILoginService loginService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<HomeController> logger
    )
    {
        _emailService = emailService;
        _configuration = configuration;
        _loginService = loginService;
        _logger = logger;
    }

    #region Login
    public IActionResult Index()
    {
        if (HttpContext.Request.Cookies["auth_token"] != null)
        {
            string? role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Admin" || role == "AccountManager")
            {
                TempData["success"] = "logged in";
                return RedirectToAction("UserDashboard", "Users");
            }else if(role == "Chef"){
                TempData["success"] = "logged in";
                return RedirectToAction("Index", "OrderApp");
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                bool response = await _loginService.GetLoginService(model);
                // if (response != null && response.token.Length > 0)
                if (response)
                {
                    // SetJwtCookie(response.token, model.Rememberme);
                    // _logger.LogInformation("User {@model.Email} logged in successfully.", model.Email);
                    // return RedirectToAction("RoleWiseBack", "Users");
                    TempData["SuccessEmail"] = "We have send you a verification code to your email";
                    return RedirectToAction("User2FAAuth", new { Email = model.Email });

                }
                TempData["EmailWrong"] = "Invalid user credentials!";
            }
            _logger.LogWarning("Invalid login attempt by {@model.Email}", model.Email);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login failed: {ex.Message}");
            TempData["ErrorMessage"] = "Login failed. Please try again later.";
            return View(model);
        }
    }


    public async Task<IActionResult> User2FAAuth(string Email)
    {
        if (HttpContext.Session.GetString("CountdownStartTime") == null)
        {
            HttpContext.Session.SetString("CountdownStartTime", DateTime.UtcNow.ToString());
        }

        var countdownStartTimeString = HttpContext.Session.GetString("CountdownStartTime");

        var countdownStartTime = countdownStartTimeString != null ? DateTime.Parse(countdownStartTimeString) : DateTime.UtcNow;

        var elapsedSeconds = (DateTime.UtcNow - countdownStartTime).TotalSeconds;

        var remainingTime = Math.Max(300 - (int)elapsedSeconds, 0); // 5 minutes in seconds
        if (remainingTime == 0)
        {
            HttpContext.Session.Remove("CountdownStartTime");
        }
        ViewBag.CountdownTime = remainingTime;

        User2FAViewModel emailViewModel = new User2FAViewModel();
    
        emailViewModel.ToEmail = Email;

        return View(emailViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> User2FAAuth(User2FAViewModel model)
    {
        try{
            if (ModelState.IsValid)
            {
                // Check if the token is valid
                ResponseTokenViewModel? isValidToken = await _loginService.Validate2faToken(model);
                if (isValidToken != null && isValidToken.token.Length > 0)
                {
                    SetJwtCookie(isValidToken.token, isValidToken.Rememberme);
                    TempData["success"] = "logged in";
                    HttpContext.Session.Remove("CountdownStartTime");
                    return RedirectToAction("RoleWiseBack", "Users");
                }
                if(isValidToken != null && isValidToken.response != "Login successful")
                {
                    if(isValidToken.response == "Token expired")
                    {
                        HttpContext.Session.Remove("CountdownStartTime");
                        return RedirectToAction("Index");
                    }

                    if (HttpContext.Session.GetString("CountdownStartTime") == null)
                    {
                        HttpContext.Session.SetString("CountdownStartTime", DateTime.UtcNow.ToString());
                    }

                    var countdownStartTimeString = HttpContext.Session.GetString("CountdownStartTime");

                    var countdownStartTime = countdownStartTimeString != null ? DateTime.Parse(countdownStartTimeString) : DateTime.UtcNow;

                    var elapsedSeconds = (DateTime.UtcNow - countdownStartTime).TotalSeconds;

                    var remainingTime = Math.Max(300 - (int)elapsedSeconds, 0); // 5 minutes in seconds
                    if (remainingTime == 0)
                    {
                        HttpContext.Session.Remove("CountdownStartTime");
                    }
                    ViewBag.CountdownTime = remainingTime;
                    TempData["ErrorMessage"] = isValidToken.response;
                    return View(model);
                }
                TempData["ErrorMessage"] = "Invalid code! Try again";
            }
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError($"2FA authentication failed: {ex.Message}");
            TempData["ErrorMessage"] = "authentication failed. Please try again later.";
            return View(model);
        }
    }

    private void SetJwtCookie(string token, bool isPersistent)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Requires HTTPS
            SameSite = SameSiteMode.Strict, // Prevent CSRF
            Expires = isPersistent ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(1),
            IsEssential = true
        };  
        Response.Cookies.Append("auth_token", token, cookieOptions);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        TempData["logout"] = "Logout Successful!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult UpdateEmail(string email)
    {
        TempData["Email"] = email;

        return Ok();
    }

    [HttpGet]
    public IActionResult ForgetPassword()
    {

        ViewBag.Email = TempData["Email"];
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgetPassword(EmailViewModel Email)
    {
            if (Email == null || string.IsNullOrEmpty(Email.ToEmail))
            {
                TempData["ErrorMessage"] = "Email address is required.";
            }
            Account? account = await _loginService.GetAccoutAsync(Email.ToEmail);
            if (account == null)
            {
                TempData["ErrorMessage"] = "No account found with this email.";
                return View(Email);
            }
            try{
                // Create new password reset request
                bool res = await _loginService.ForgetPasword(Email.ToEmail);

                if(res == false)
                {
                    TempData["ErrorMessage"] = "No account found with this email.";
                    return View(Email);
                }
                _logger.LogInformation("Password reset request created for {@Email}", Email.ToEmail);
                TempData["validEmail"] = Email.ToEmail;
                TempData["SuccessMessage"] = "Password reset instructions have been sent to your email.";
                TempData.Keep("validEmail");
                return View(Email);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                _logger.LogError($"Failed to send email to {@Email.ToEmail}: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to send email. Please try again later.";
                return View(Email);
            }
    }

    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = "Invalid password reset link.";
            return RedirectToAction("ForgetPassword");
        }

            PasswordResetRequest? resetRequest = await _loginService.ResetPasswordGetService(token);
    
                if (resetRequest == null)
                {
                    TempData["ErrorMessage"] = "Invalid or expired password reset link.";
                    return RedirectToAction("ForgetPassword");
                }

                // Check if request is still open (CloseDate is null)
                if (resetRequest.Closedate != null)
                {
                    TempData["ErrorMessage"] = "This password reset link has been expire.";
                    return RedirectToAction("ForgetPassword");
                }

                // Check if request is within 24 hours
                if (resetRequest.Createdate < DateTime.Now.AddHours(-24))
                {
                    TempData["ErrorMessage"] = "This password reset link has expired.";
                    return RedirectToAction("ForgetPassword");
                }

                // Get the email from UserId
                Account? account = await _loginService.GetAccountById(resetRequest.Userid);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return RedirectToAction("ForgetPassword");
                }

        return View(new ForgetPasswordViewModel { Token = token, Email = account.Email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ForgetPasswordViewModel model)
    {
        try{
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            PasswordResetRequest? resetRequest = await _loginService.ResetPasswordGetService(model.Token);

            if (resetRequest == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired password reset link.";
                return RedirectToAction("ForgetPassword");
            }

            // Check if request is still open
            if (resetRequest.Closedate != null)
            {
                TempData["ErrorMessage"] = "This password reset link has already been used.";
                return RedirectToAction("ForgetPassword");
            }

            // Check if request is within 24 hours
            if (resetRequest.Createdate < DateTime.Now.AddHours(-24))
            {
                TempData["ErrorMessage"] = "This password reset link has expired.";
                return RedirectToAction("ForgetPassword");
            }


            if (ModelState.IsValid)
            {
                string? response = await _loginService.ResetPasswordService(model);

                switch (response)
                {
                    case "1":
                        TempData["password"] = "account does not exist";
                        break;
                    case "2":
                        TempData["EmailNotMatch"] = "email doesnot match";
                        break;
                    case "3":
                        TempData["password"] = "password can not be same as previous one";
                        break;
                    case "4":
                        TempData["success"] = "successfully changed password";
                        return RedirectToAction("Index");
                    default:
                        TempData["password"] = "invalid";
                        break;
                }
            }
            TempData["error"] = "please confirm password first";
            return View(model);
        }catch (Exception ex)
        {
            Console.WriteLine($"Failed to reset password: {ex.Message}");
            TempData["ErrorMessage"] = "Failed to reset password. Please try again later.";
            return View(model);
        }
    }

    #endregion
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error404()
    {
        return View();
    }

   
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
