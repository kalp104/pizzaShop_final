using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public TokenMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var publicPaths = new[]
        {
            "/Home/Index",
            "/Home/Logout",
            "/Home/ForgetPassword",
            "/Home/ResetPassword",
            "/Home/UpdateEmail",
            "/Home/User2FAAuth"
        };

        var currentPath = context.Request.Path.ToString();
        bool isPublicPath = publicPaths.Any(path =>
            currentPath.StartsWith(path, StringComparison.OrdinalIgnoreCase)
        );

        if (
            !context.Request.Cookies.TryGetValue("auth_token", out var token)
            || string.IsNullOrEmpty(token)
        )
        {
            if (!isPublicPath)
            {
                context.Response.Redirect("/Home/Index");
                return;
            }
            await _next(context);
            return;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out _);
            context.User = principal;

            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(emailClaim) || string.IsNullOrEmpty(roleClaim))
            {
                Console.WriteLine("Email or Role not found in token claims.");
                if (!isPublicPath)
                {
                    context.Response.Redirect("/Home/Index");
                    return;
                }
            }
            else
            {
                context.Items["UserEmail"] = emailClaim;
                context.Items["UserRole"] = roleClaim;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            if (!isPublicPath)
            {
                context.Response.Redirect("/Home/Index");
                return;
            }
        }

        await _next(context);
    }
}

public static class TokenMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenMiddleware>();
    }
}
