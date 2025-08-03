using System;
using System.Collections.Specialized;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class SectionService : ISectionService
{
    
    private readonly IWaitingListRepository _waitingListRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IOrderRepository _orderRepository;


        public SectionService(
        IWaitingListRepository waitingListRepository,
        ITableRepository tableRepository,
        ISectionRepository sectionRepository,
        IOrderRepository orderRepository
    )
    {
        _waitingListRepository = waitingListRepository;
        _tableRepository = tableRepository;
        _sectionRepository = sectionRepository;
        _orderRepository = orderRepository;
    }

    // index page get all section service
    public async Task<SectionsHelperViewModel> GetSectionsAsync(
        int? sectionId = null,
        string? searchTable = null,
        int pageNumber = 1,
        int pageSize = 5
    )
    {
        List<Section>? s = await _sectionRepository.GetAllSections();
        

        List<Table>? tables;
        if (sectionId.HasValue)
        {
            tables = await _tableRepository.GetTableBySectionIdAsync(sectionId);
        }
        else
        {
            tables = await _tableRepository.GetAllTables();
        }

        if (!string.IsNullOrEmpty(searchTable) && tables!=null)
        {
            searchTable = searchTable.ToLower();
            tables = tables.Where(u => u.Tablename.ToLower().Contains(searchTable)).ToList();
        }

        int totalTales = tables.Count;
        tables = tables.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        SectionsHelperViewModel section = new SectionsHelperViewModel();
        section.Sections = s;
        section.Tables = tables;
        section.CurrentPage = pageNumber;
        section.PageSize = pageSize;
        section.TotalTables = totalTales;
        section.Sectionid = sectionId ?? 0;
        return section;
    }

    // index page add section service
    public async Task<bool> AddSectionService(SectionsHelperViewModel model)
    {
        try{
            List<Section>? checkSection = await _sectionRepository.CheckSectionByName(model.Sectionname.Trim().ToLower());
            if(checkSection != null && checkSection.Count > 0)
            {
                return false;
            }

            Section section = new Section
            {
                Sectionname = model.Sectionname,
                Sectiondescription = model.Sectiondescription,
                Isdeleted = false,
                Createdat = DateTime.Now,
                Createdbyid = model.Userid,
                Editedat = DateTime.Now,
                Editedbyid = model.Userid,
            };
            await _sectionRepository.AddSection(section);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue in add section ");
        }

        
    }

    // index page edit section service
    public async Task<bool> EditSectionService(SectionsHelperViewModel model)
    {
        try{
            List<Section>? checkSection = await _sectionRepository.CheckSectionByName(model.Sectionname.Trim().ToLower());
            if(checkSection != null && checkSection.Count > 0)
            {
                foreach(Section s in checkSection)
                {
                    if(s.Sectionid!=model.Sectionid && model.Sectionname.Trim().ToLower().Equals(s.Sectionname.Trim().ToLower())){
                        return false;
                    }
                }
            }

            Section? section = await _sectionRepository.GetSectionById(model.Sectionid);
            if (section != null)
            {
                section.Sectiondescription = model.Sectiondescription;
                section.Sectionname = model.Sectionname;
                section.Editedat = DateTime.Now;
                section.Editedbyid = model.Userid;
                await _sectionRepository.UpdateSection(section);
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue in edit section ");
        }
        
    }

    //index page delete section service
    public async Task<bool> DeleteSectionService(SectionsHelperViewModel model)
    {
        try{
            Section? section = await _sectionRepository.GetSectionById(model.Sectionid);
            if (section == null)
            {
                return false;
            }

            List<Table>? tables = await _tableRepository.GetTableBySectionIdAsync(model.Sectionid);
            if (tables != null && tables.Any(t => t.Status >= 2 && t.Isdeleted == false))
            {
                return false;
            }

            section.Isdeleted = true;
            section.Deletedat = DateTime.Now;
            section.Deletedbyid = model.Userid;
            await _sectionRepository.UpdateSection(section);

            if (tables != null)
            {
                foreach (var table in tables)
                {
                    table.Isdeleted = true;
                    table.Deletedat = DateTime.Now;
                    table.Deletedbyid = model.Userid;
                    await _tableRepository.UpdateTable(table);
                }
            }

            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue in delete section ");
        }
    }

    // table crud
    // add table service
    public async Task<bool> AddTableService(SectionsHelperViewModel model)
    {
        try{
            if (string.IsNullOrWhiteSpace(model?.Tablename))
            {
                throw new ArgumentException("Table name cannot be null or empty.");
            }
            List<Table>? checkTables = await _tableRepository.CheckTableByName(model.Tablename.Trim().ToLower());
            if(checkTables != null && checkTables.Count > 0)
            {
                return false;
            }

            Table table = new Table
            {
                Sectionid = model.Sectionid,
                Tablename = model.Tablename,
                Capacity = model.Capacity ?? 0,
                Status = model.Status ?? 0,
                Isdeleted = false,
                Createdat = DateTime.Now,
                Createdbyid = model.Userid,
                Editedat = DateTime.Now,
                Editedbyid = model.Userid,
            };
            await _tableRepository.AddTable(table);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue add table");
        }
        
    }

    // delete table service
    public async Task<bool> DeleteTableService(int tableId, int userId)
    {
        try{
            Table? table = await _tableRepository.GetTablesById(tableId);
            if (table == null)
            {
                return false;
            }

            if (table.Status >= 2)
            {
                return false;
            }

            table.Isdeleted = true;
            table.Deletedat = DateTime.Now;
            table.Deletedbyid = userId;
            await _tableRepository.UpdateTable(table);
            return true; // Deletion successful
        }catch (Exception e){
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue delete table");
        }
    }

    // delete multiple tables
    public async Task<bool> DeleteMultipleTablesService(List<int> tableIds, int userId)
    {
        try{
            
            foreach(int id in tableIds)
            {
                Table? table = await _tableRepository.GetTablesById(id);
                if(table==null || table.Status >= 2)
                {
                    return false;
                }
            }

            foreach(int id in tableIds)
            {
                Table? table = await _tableRepository.GetTablesById(id);
                if(table!=null)
                {
                    table.Isdeleted = true;
                    table.Deletedat = DateTime.Now;
                    table.Deletedbyid = userId;
                    await _tableRepository.UpdateTable(table);
                }
            }

            return true;
        }catch(Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    // edit table service
    public async Task<bool> EditTableService(SectionsHelperViewModel model)
    {
        try{
            if (string.IsNullOrWhiteSpace(model?.Tablename))
            {
                throw new ArgumentException("Table name cannot be null or empty.");
            }
            List<Table>? checkTables = await _tableRepository.CheckTableByName(model.Tablename.Trim().ToLower());
            if(checkTables != null && checkTables.Count > 0)
            {
               foreach(Table s in checkTables)
                {
                    if(s.Tableid!=model.Tableid && model.Tablename.Trim().ToLower().Equals(s?.Tablename.Trim().ToLower())){
                        return false;
                    }
                }
            }

            Table? table = await _tableRepository.GetTablesById(model.Tableid);
            if (table != null)
            {
                table.Tablename = model.Tablename;
                table.Capacity = model.Capacity ?? 0;
                table.Status = model.Status ?? 0;
                table.Editedat = DateTime.Now;
                table.Editedbyid = model.Userid;
                await _tableRepository.UpdateTable(table);
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Error-500 : issue edit table");
        }
    }

    // section and table for order app

    public async Task<OrderAppTableViewModel> GetAllSections()
    {
        List<Section>? sections = await _sectionRepository.GetAllSections();
        
        OrderAppTableViewModel orderAppTableViewModel = new();
        List<SectionsOrderAppViewModel> sectionsOrderAppViewModel = new();
        if (sections != null)
        {
            foreach (Section s in sections)
            {
                SectionsOrderAppViewModel newSection = new()
                {
                    Sectiondescription = s.Sectiondescription,
                    Sectionid = s.Sectionid,
                    Sectionname = s.Sectionname,
                };
                sectionsOrderAppViewModel.Add(newSection);

                int Available = 0;
                int Assigned = 0;
                int Running = 0;
                List<Table>? tables = await _tableRepository.GetTableBySectionIdAsync(s.Sectionid);
                List<TableOrderAppViewModel> tableOrderAppViewModel = new();

                if(tables!=null)
                {
                    foreach (Table t in tables)
                    {
                        if (t.Status == 1)
                        {
                            Available++;
                        }
                        else if (t.Status == 2)
                        {
                            Assigned++;
                        }
                        else if (t.Status == 3)
                        {
                            Running++;
                        }

                        OrdersTablesMapping? ots = await _tableRepository.GetOrderByTableId(t.Tableid);

                        if (t.Status == 3 && ots != null)
                        {
                            Order? order = await _orderRepository.GetOrderById(ots.Orderid);
                            if (order != null)
                            {
                                TableOrderAppViewModel newTable = new()
                                {
                                    Amount = order.Totalamount,
                                    Tableid = t.Tableid,
                                    Tablename = t.Tablename ?? "N/A",
                                    Status = t.Status,
                                    Sectionid = s.Sectionid,
                                    TCapacity = t.Capacity,
                                    
                                };


                                TimeSpan DateDifference = order.Createdat.HasValue 
                                ? (DateTime.Now - order.Createdat.Value) 
                                : TimeSpan.Zero;
                                string DateFormate = (DateDifference.Days > 0 ? $"{DateDifference.Days} Days " : "")
                                            +   (DateDifference.Hours > 0 ? $"{DateDifference.Hours} Hours " : "") 
                                            +   (DateDifference.Minutes > 0 ? $"{DateDifference.Minutes} Min " : ""); 

                                newTable.OrderTime = DateFormate;

                                tableOrderAppViewModel.Add(newTable);
                            }
                        }
                        else
                        {
                            TableOrderAppViewModel newTable = new()
                            {
                                Amount = 0,
                                Tableid = t.Tableid,
                                Tablename = t.Tablename ?? "N/A",
                                Status = t.Status,
                                Sectionid = s.Sectionid,
                                TCapacity = t.Capacity,
                            };
                            tableOrderAppViewModel.Add(newTable);
                        }
                    }
                }

                if (tables != null)
                {
                    newSection.Available = Available;
                    newSection.Assigned = Assigned;
                    newSection.Running = Running;
                    newSection.tables = tableOrderAppViewModel;
                }
            }

            orderAppTableViewModel.sections = sectionsOrderAppViewModel;
        }
        return orderAppTableViewModel;
    }

    public async Task<bool> AddWaitingToken(OrderAppTableViewModel model, int userId)
    {
        if (model != null)
        {
            WaitingList waitingList = new WaitingList();
            waitingList.Sectionid = model.Sectionid;
            waitingList.Customername = model.Customername;
            waitingList.Customeremail = model.Customeremail;
            waitingList.Customerphone = model.Customerphone ?? 0;
            waitingList.Totalperson = model.TotalPersons ?? 0;
            waitingList.Createdat = DateTime.Now;
            waitingList.Createdbyid = userId;
            waitingList.Editedat = DateTime.Now;
            waitingList.Editedbyid = userId;
            waitingList.Isdeleted = false;
            await _waitingListRepository.AddWaitingList(waitingList);
        }

        return true;
    }
}
