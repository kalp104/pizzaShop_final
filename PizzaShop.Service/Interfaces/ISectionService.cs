using System;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Service.Interfaces;

public interface ISectionService
{
    // section crud
    Task<SectionsHelperViewModel> GetSectionsAsync(
        int? sectionId = null,
        string? searchTable = null,
        int pageNumber = 1,
        int pageSize = 5
    );
    Task<bool> AddSectionService(SectionsHelperViewModel model);
    Task<bool> EditSectionService(SectionsHelperViewModel model);
    Task<bool> DeleteSectionService(SectionsHelperViewModel model);

    // table crud
    Task<bool> AddTableService(SectionsHelperViewModel model);
    Task<bool> DeleteTableService(int tableId, int userId);
    Task<bool> DeleteMultipleTablesService(List<int> tableIds, int userId);
    Task<bool> EditTableService(SectionsHelperViewModel model);

    // for order app
    Task<OrderAppTableViewModel> GetAllSections();
    Task<bool> AddWaitingToken(OrderAppTableViewModel model, int userId);
}
