using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Core;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;
using System; 
using System.Data; 
using Npgsql; 
using Dapper;
using Serilog;
using PizzaShop.Core.hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserTableService, UserTableService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ITaxService, TaxService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderAppService, OrderAppService>();

//excel
OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

// Filters
builder.Services.AddScoped<AuthorizePermissionUserTable>();
builder.Services.AddScoped<AuthorizePermissionRoles>();
builder.Services.AddScoped<AuthorizePermissionMenu>();
builder.Services.AddScoped<AuthorizePermissionSections>();
builder.Services.AddScoped<AuthorizePermissionTax>();
builder.Services.AddScoped<AuthorizePermissionOrders>();
builder.Services.AddScoped<AuthorizePermissionCustomer>();
builder.Services.AddScoped<AuthorizePermissionOrderApp>();
builder.Services.AddScoped<AuthorizePermissionUser>();


// Connection string + dependency injection
builder.Services.AddDbContext<PizzaShop2Context>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// serilog - Logger
Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt")
            .CreateLogger();


builder.Host.UseSerilog();

// npg sql connection for dapper
builder.Services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ICustomerRepository), typeof(CustomerRepository));
builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
builder.Services.AddScoped(typeof(ILoginRepository), typeof(LoginRepository));
builder.Services.AddScoped(typeof(IRoleRepository), typeof(RoleRepository));
builder.Services.AddScoped(typeof(ITaxRepository), typeof(TaxRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IWaitingListRepository), typeof(WaitingListRepository));
builder.Services.AddScoped(typeof(ISectionRepository), typeof(SectionRepository));
builder.Services.AddScoped(typeof(ITableRepository), typeof(TableRepository));
builder.Services.AddScoped(typeof(IItemRepository), typeof(ItemRepository));
builder.Services.AddScoped(typeof(IItemRepository), typeof(ItemRepository));
builder.Services.AddScoped(typeof(IModifierRepository), typeof(ModifierRepository));
builder.Services.AddScoped(typeof(ICategoryRepository), typeof(CategoryRepository));
builder.Services.AddScoped(typeof(IModifierGroupRepository), typeof(ModifierGroupRepository));
builder.Services.AddScoped(typeof(IFeedBackRepository), typeof(FeedBackRepository));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    // Read JWT from cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["auth_token"];
            return Task.CompletedTask;
        }
    };
});


// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PublicAccess", policy => policy.RequireAssertion(context => true)); // Allow all
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("AccountManagerOnly", policy => policy.RequireRole("AccountManager"));
    options.AddPolicy("ChefOnly", policy => policy.RequireRole("Chef"));
    
});

// Add HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Privacy"); // General error page for unhandled exceptions
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


// Add session middleware
app.UseSession();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/Home/Index"); // Redirect to login for unauthenticated
    }
    else if (context.HttpContext.Response.StatusCode == 403)
    {
        context.HttpContext.Response.Redirect("/Home/Error403"); // Custom 403 page
    }
    else if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/Home/Error404");
    }
    await Task.CompletedTask;
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<OrderAppHub>("/OrderAppHub");

app.Run();


