using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Core.Filters;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;
using System.Linq;
using System.Security.Claims;

namespace PizzaShop.Core.Controllers;

[Authorize]
[ServiceFilter(typeof(AuthorizePermissionSections))]
public class SectionController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IUserTableService _userTableService;
    private readonly IRoleService _roleService;
    private readonly ISectionService _sectionService;

    public SectionController(
        IUserTableService userTableService,
        IConfiguration configuration,
        IUserService userService,
        IRoleService roleService,
        ISectionService sectionService
    )
    {
        _configuration = configuration;
        _userService = userService;
        _userTableService = userTableService;
        _roleService = roleService;
        _sectionService = sectionService;
    }

    private async Task FetchData()
    {
        string? role = User.FindFirst(ClaimTypes.Role)?.Value;
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        UserBagViewModel? userBag = await _userService.UserDetailBag(email);
        List<RolePermissionModelView>? rolefilter = await _userService.RoleFilter(role);
        if (userBag != null)
        {
            ViewBag.role = role;
            ViewBag.Username = userBag.UserName;
            ViewBag.Userid = userBag.UserId;
            ViewBag.ImageUrl = userBag.ImageUrl;
            ViewBag.permission = rolefilter;
        }
    }

    // Index of section and tables
    public async Task<IActionResult> Index(
        int? sectionId = null,
        string? searchTable = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        SectionsHelperViewModel section = await _sectionService.GetSectionsAsync(
            sectionId,
            searchTable,
            pageNumber,
            pageSize
        );
        ViewBag.SeclectedSectionId = section.Sectionid;
        return View(section);
    }

    #region Section
    // Fetch sections for AJAX
    [HttpGet]
    public async Task<IActionResult> GetSections()
    {
        var sectionsModel = await _sectionService.GetSectionsAsync();
        var sections = sectionsModel.Sections.Select(s => new
        {
            sectionid = s.Sectionid,
            sectionname = s.Sectionname,
            sectiondescription = s.Sectiondescription
        }).ToList();
        return Json(new { sections });
    }

    // Section CRUD
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSection(SectionsHelperViewModel model){
        try{

            await FetchData();
            bool res = await _sectionService.AddSectionService(model);
            if (res)
            {
                return Json(new { success = true, message = "Section added successfully" });
            }
            return Json(new { success = false, message = "section name already exist! please enter valid details" });
        }
        catch(Exception e)
        {
             return Json(new { success = false, message = e.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSection(SectionsHelperViewModel model){
        try{
            await FetchData();
            bool res = await _sectionService.EditSectionService(model);
            if (res)
            {
                return Json(new { success = true, message = "Section edited successfully" });
            }
            return Json(new { success = false, message = "section name already exist! please enter valid details" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSection(SectionsHelperViewModel model){
        try{
            bool success = await _sectionService.DeleteSectionService(model);
            if (success)
            {
                return Json(new { success = true, message = "Section deleted successfully" });
            }
            return Json(new { success = false, message = "Cannot delete section because some tables are occupied." });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    #endregion
    #region Tables

    // Table CRUD
    [HttpGet]
    public async Task<IActionResult> FilterTables(
        int? sectionId = null,
        string? searchTable = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        await FetchData();
        SectionsHelperViewModel section = await _sectionService.GetSectionsAsync(
            sectionId,
            searchTable,
            pageNumber,
            pageSize
        );
        ViewBag.SeclectedSectionId = section.Sectionid;
        return PartialView("_TablesPartial", section);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTable(SectionsHelperViewModel model){
        try{
            await FetchData();
            bool res = await _sectionService.AddTableService(model);
            if (res)
            {
                return Json(new { success = true, message = "Table added successfully" });
            }
            return Json(new { success = false, message = "table name already exist! please enter valid details" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message});
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTable(int Tableid){
        try{
            await FetchData();
            int userid = ViewBag.Userid;
            bool success = await _sectionService.DeleteTableService(Tableid, userid);
            if (success)
            {
                return Json(new { success = true, message = "Table deleted successfully" });
            }
            return Json(new { success = false, message = "Cannot delete table because it is occupied." });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTable(SectionsHelperViewModel model)
    {
        try{
            await FetchData();
            bool res = await _sectionService.EditTableService(model);
            if (res)
            {
                return Json(new { success = true, message = "Table edited successfully" });
            }
            return Json(new { success = false, message = "this table already exists! please enter valid details" });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTables(string deleteSelectedTableIds)
    {
        await FetchData();
        int userid = ViewBag.Userid;

        if (string.IsNullOrEmpty(deleteSelectedTableIds))
        {
            return Json(new { success = false, message = "No tables selected for deletion." });
        }

        try{
            List<int> tableIds = deleteSelectedTableIds.Split(',').Select(int.Parse).ToList();
                        
            bool success = await _sectionService.DeleteMultipleTablesService(tableIds, userid);
            if (success)
            {
                return Json(new { success = true, message = "Tables deleted successfully" });
            }

            return Json(new { success = false, message = "Some tables could not be deleted because they are occupied." });
        }
        catch (Exception e)
        {
             return Json(new { success = false, message = "Error 500 : internal server error " });
        }
    }

    #endregion
}