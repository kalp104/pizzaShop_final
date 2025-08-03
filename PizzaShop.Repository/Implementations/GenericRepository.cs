using Microsoft.EntityFrameworkCore;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Repository.ModelView;

namespace PizzaShop.Repository.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly PizzaShop2Context _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(PizzaShop2Context context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<Account?> GetAccountByEmail(string email)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(u => u.Email == email && u.Isdeleted == false);
        }
        public async Task<Account?> GetAccountByUsername(string username)
        {
            return await _dbSet.OfType<Account>().FirstOrDefaultAsync(u => u.Username == username)
                ?? null;
        }

        public async Task<string?> UpdateAsync(T model)
        {
            if (model != null)
            {
                _dbSet.Update(model);
                await _context.SaveChangesAsync();
                return "saved";
            }
            return "unSaved";
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbSet.OfType<User>().FirstOrDefaultAsync(u => u.Accountid == id);
        }

        public async Task<List<RolePermissionModelView>?> GetRolePermissionModelViewAsync(
            int roleid
        )
        {
            // List<RolePermissionModelView> result = (
            //     from pr in _context.PermissionsRoles
            //     join r in _context.Roles on pr.Roleid equals r.Roleid
            //     join p in _context.Permissions on pr.Permissionid equals p.Permissionid
            //     where r.Roleid == roleid
            //     select new RolePermissionModelView
            //     {
            //         RoleId = r.Roleid,
            //         PermissionId = p.Permissionid,
            //         Canview = (bool)pr.Canview,
            //         Canedit = (bool)pr.Canedit,
            //         Candelete = (bool)pr.Candelete,
            //     }
            // ).ToList();

            List<RolePermissionModelView> result = await _context.PermissionsRoles
                .Where(pr => pr.Roleid == roleid)
                .Join(_context.Roles, pr => pr.Roleid, r => r.Roleid, (pr, r) => new { pr, r })
                .Join(_context.Permissions, pr_r => pr_r.pr.Permissionid, p => p.Permissionid, (pr_r, p) => new RolePermissionModelView
                {
                    RoleId = pr_r.r.Roleid,
                    PermissionId = p.Permissionid,
                    Canview = (bool)pr_r.pr.Canview,
                    Canedit = (bool)pr_r.pr.Canedit,
                    Candelete = (bool)pr_r.pr.Candelete,
                })
                .ToListAsync();
            return result;
        }

        public async Task<RolePermissionModelView?> GetPermissionAsync(int roleid, int permission)
        {
            // RolePermissionModelView? result = (
            //     from pr in _context.PermissionsRoles
            //     join r in _context.Roles on pr.Roleid equals r.Roleid
            //     join p in _context.Permissions on pr.Permissionid equals p.Permissionid
            //     where r.Roleid == roleid && p.Permissionid == 1
            //     select new RolePermissionModelView
            //     {
            //         RoleId = roleid,
            //         PermissionId = p.Permissionid,
            //         Canview = (bool)pr.Canview,
            //         Canedit = (bool)pr.Canedit,
            //         Candelete = (bool)pr.Candelete,
            //     }
            // ).FirstOrDefault();

            // RolePermissionModelView? result = await _context.PermissionsRoles
            //     .Where(pr => pr.Roleid == roleid && pr.Permissionid == permission)
            //     .Join(_context.Roles, pr => pr.Roleid, r => r.Roleid, (pr, r) => new { pr, r })
            //     .Join(_context.Permissions, pr_r => pr_r.pr.Permissionid, p => p.Permissionid, (pr_r, p) => new RolePermissionModelView
            //     {
            //         RoleId = pr_r.r.Roleid,
            //         PermissionId = p.Permissionid,
            //         Canview = (bool)pr_r.pr.Canview,
            //         Canedit = (bool)pr_r.pr.Canedit,
            //         Candelete = (bool)pr_r.pr.Candelete,
            //     })
            //     .FirstOrDefaultAsync();
            // return result;

            RolePermissionModelView? result = await _context.PermissionsRoles
                .Include(pr => pr.Role)
                .Include(pr => pr.Permission)
                .Where(pr => pr.Roleid == roleid && pr.Permissionid == permission)
                .Select(pr => new RolePermissionModelView
                {
                    RoleId = pr.Role.Roleid,
                    PermissionId = pr.Permission.Permissionid,
                    Canview = (bool)pr.Canview,
                    Canedit = (bool)pr.Canedit,
                    Candelete = (bool)pr.Candelete,
                })
                .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<UserTableViewModel>?> UserDetailAsync()
        {
            return await (
                from u in _context.Users
                join a in _context.Accounts on u.Accountid equals a.Accountid
                join r in _context.Roles on u.Roleid equals r.Roleid
                where u.Isdeleted == false && a.Isdeleted == false
                orderby u.Userid descending
                select new UserTableViewModel
                {
                    AccountId = a.Accountid,
                    Id = u.Userid,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Email = a.Email,
                    Phone = u.Phone,
                    Role = a.Roleid,
                    Rolename = r.Rolename,
                    Status = u.Status,
                    Image = u.Userimage
                }
            ).ToListAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int? id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<City>> GetCityListByIdAsync(int id)
        {
            return await _context.Cities.Where(b => b.Stateid == id).ToListAsync();
        }

        public async Task<List<State>> GetStateListByIdAsync(int id)
        {
            return await _context.States.Where(b => b.Countryid == id).ToListAsync();
        }

        public async Task AddAsync(T model)
        {
            _dbSet.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RolePermissionModelView>?> GetPermissionAsync(int id)
        {
            var result = await (
                from pr in _context.PermissionsRoles
                join r in _context.Roles on pr.Roleid equals r.Roleid
                join p in _context.Permissions on pr.Permissionid equals p.Permissionid
                where r.Roleid == id
                orderby pr.Permissionroleid
                select new RolePermissionModelView
                {
                    RoleId = id,
                    PermissionId = p.Permissionid,
                    RoleName = r.Rolename,
                    PermissionName = p.Permissionname,
                    Canview = (bool)pr.Canview,
                    Canedit = (bool)pr.Canedit,
                    Candelete = (bool)pr.Candelete,
                }
            ).ToListAsync();
            return result;
        }

        public async Task<PermissionsRole?> GetRoleAndPermissionAsync(int roleid, int permissionid)
        {
            return await _dbSet
                .OfType<PermissionsRole>()
                .FirstOrDefaultAsync(p => p.Roleid == roleid && p.Permissionid == permissionid);
        }

        public async Task<List<Category>> GetAllCategoryAsync()
        {
            return await _context
                .Categories.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Categoryid)
                .ToListAsync();
        }

        public async Task<List<Modifiergroup>> GetAllModifierGroupAsync()
        {
            return await _context
                .Modifiergroups.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Modifiergroupid)
                .ToListAsync();
        }

        public async Task<List<Item>> GetAllItemsAsync()
        {
            return await _context
                .Items.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Itemid)
                .ToListAsync();
        }

        public async Task<List<Modifier>> GetAllModifierAsync()
        {
            return await _context
                .Modifiers.Where(u => u.Isdeleted == false)
                .OrderBy(u => u.Modifierid)
                .ToListAsync();
        }

        public async Task<List<Item>> GetItemsByCategoryAsync(int? id)
        {
            return await _context
                .Items.Where(u => u.Isdeleted == false && u.Categoryid == id)
                .OrderBy(u => u.Itemid)
                .ToListAsync();
        }

        public async Task<List<Table>> GetTableBySectionIdAsync(int? id)
        {
            return await _context
                .Tables.Where(u => u.Isdeleted == false && u.Sectionid == id)
                .OrderBy(u => u.Tableid)
                .ToListAsync();
        }

        public async Task<List<Modifier>> GetModifiersByMGAsync(int? id)
        {
            // return await _context.Modifiers.Where(u => u.Isdeleted == false && u.Modifiergroupid == id ).OrderBy(u => u.Modifierid).ToListAsync();

            var result = await (
                from mgm in _context.Modfierandgroupsmappings
                join m in _context.Modifiers on mgm.Modifierid equals m.Modifierid
                where mgm.Modifiergroupid == id
                where m.Isdeleted == false
                orderby m.Modifierid
                select new Modifier
                {
                    Modifierid = m.Modifierid,
                    Modifiername = m.Modifiername,
                    Modifierrate = m.Modifierrate,
                    Modifierquantity = m.Modifierquantity,
                    Modifierunit = m.Modifierunit,
                    Modifierdescription = m.Modifierdescription,
                    Taxpercentage = m.Taxpercentage,
                    Isdeleted = m.Isdeleted,
                    Createdat = m.Createdat,
                    Deletedat = m.Deletedat,
                    Editedat = m.Editedat,
                    Editedbyid = m.Editedbyid,
                    Createdbyid = m.Createdbyid,
                    Deletedbyid = m.Deletedbyid,
                }
            ).ToListAsync();
            return result;
        }

        public async Task<Modfierandgroupsmapping?> GetMappingsById(
            int Modifiergroupid,
            int modifierid
        )
        {
            return await _context
                .Modfierandgroupsmappings.Where(u =>
                    u.Modifierid == modifierid && u.Modifiergroupid == Modifiergroupid
                )
                .FirstOrDefaultAsync();
        }

        public async Task<List<Modfierandgroupsmapping>?> GetByModifierIdAsync(int? modifiersid)
        {
            return await _context
                .Modfierandgroupsmappings.Where(u => u.Modifierid == modifiersid)
                .ToListAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Modfierandgroupsmapping>?> GetByModifierGroupIdAsync(
            int? modifierGroupId
        )
        {
            return await _context
                .Modfierandgroupsmappings.Where(u => u.Modifiergroupid == modifierGroupId)
                .ToListAsync();
        }

        // ItemModifiergroupMapping
        public async Task<List<ItemModifiergroupMapping>?> GetByItemIdAsync(int? itemId)
        {
            return await _context
                .ItemModifiergroupMappings.Where(u => u.Itemid == itemId && u.Isdeleted == false)
                .ToListAsync();
        }

        public async Task<List<ItemModifiergroupMapping>?> GetByModifierGroupIdMappingAsync(
            int? modifierGroupId
        )
        {
            return await _context
                .ItemModifiergroupMappings.Where(u => u.Modifiergroupid == modifierGroupId)
                .ToListAsync();
        }

        public async Task<List<OrderCutstomerViewModel>?> GetAllCustomerOrderMappingAsync()
        {
            //  var result = await (
            //     from mapping in _context.OrdersCustomersMappings
            //     join c in _context.Customers on mapping.Customerid equals c.Customerid
            //     join o in _context.Orders on mapping.Orderid equals o.Orderid into ordersGroup
            //     from o in ordersGroup.DefaultIfEmpty()
            //     where c.Isdeleted == false
            //     where o == null || o.Isdeleted == false
            //     select new OrderCutstomerViewModel
            //     {
            //         Orderid = o != null ? o.Orderid : 0,
            //         Customerid = c.Customerid,
            //         Orderdescription = o != null ? o.Orderdescription : null,
            //         Createdat = o != null ? o.Createdat : null,
            //         Status = o != null ? o.Status : 0,
            //         Paymentmode = o != null ? o.Paymentmode : 0,
            //         Ratings = o != null ? o.Ratings : null,
            //         Totalamount = o != null ? o.Totalamount : 0,
            //         Customername = c.Customername,
            //         Customeremail = c.Customeremail,
            //         Customerphone = c.Customerphone,
            //     }
            // ).ToListAsync();

            
            // var result = await _context.OrdersCustomersMappings
            //     .Join(_context.Customers, mapping => mapping.Customerid, c => c.Customerid, (mapping, c) => new { mapping, c })
            //     .GroupJoin(_context.Orders, mc => mc.mapping.Orderid, o => o.Orderid, (mc, ordersGroup) => new { mc, ordersGroup })
            //     .SelectMany(
            //         mc_o => mc_o.ordersGroup.DefaultIfEmpty(),
            //         (mc_o, o) => new OrderCutstomerViewModel
            //         {
            //             Orderid = o != null ? o.Orderid : 0,
            //             Customerid = mc_o.mc.c.Customerid,
            //             Orderdescription = o != null ? o.Orderdescription : null,
            //             Createdat = o != null ? o.Createdat : null,
            //             Status = o != null ? o.Status : 0,
            //             Paymentmode = o != null ? o.Paymentmode : 0,
            //             Ratings = o != null ? o.Ratings : null,
            //             Totalamount = o != null ? o.Totalamount : 0,
            //             Customername = mc_o.mc.c.Customername,
            //             Customeremail = mc_o.mc.c.Customeremail,
            //             Customerphone = mc_o.mc.c.Customerphone,
            //         }
            //     )
            //     .Where(viewModel => viewModel.Customerid != 0 && (viewModel.Orderid == 0 || viewModel.Status != 0))
            //     .ToListAsync();
            var result = await _context.OrdersCustomersMappings
                .Include(mapping => mapping.Customer)
                .Include(mapping => mapping.Order)
                .Where(mapping => mapping.Customer.Isdeleted == false && 
                                  (mapping.Order == null || mapping.Order.Isdeleted == false))
                .Select(mapping => new OrderCutstomerViewModel
                {
                    Orderid = mapping.Order != null ? mapping.Order.Orderid : 0,
                    Customerid = mapping.Customer.Customerid,
                    Orderdescription = mapping.Order != null ? mapping.Order.Orderdescription : null,
                    Createdat = mapping.Order != null ? mapping.Order.Createdat : null,
                    Status = mapping.Order != null ? mapping.Order.Status : 0,
                    Paymentmode = mapping.Order != null ? mapping.Order.Paymentmode : 0,
                    Ratings = mapping.Order != null ? mapping.Order.Ratings : null,
                    Totalamount = mapping.Order != null ? mapping.Order.Totalamount : 0,
                    Customername = mapping.Customer.Customername,
                    Customeremail = mapping.Customer.Customeremail,
                    Customerphone = mapping.Customer.Customerphone,
                })
                .Where(viewModel => viewModel.Customerid != 0 && (viewModel.Orderid == 0 || viewModel.Status != 0))
                .ToListAsync();
            return result;
        }

        public async Task<OrderDetailsHelperViewModel?> GetOrderDetailsByOrderId(int orderId)
        {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid
                where c.Isdeleted == false
                where o.Isdeleted == false
                where o.Orderid == orderId
                orderby o.Createdat descending
                select new OrderDetailsHelperViewModel
                {
                    Orderid = o.Orderid,
                    Customerid = c.Customerid,
                    Orderdescription = o.Orderdescription,
                    Createdat = o.Createdat,
                    CompletedAt = o.Completedat,
                    Status = o.Status,
                    Paymentmode = o.Paymentmode,
                    Ratings = o.Ratings,
                    Totalamount = o.Totalamount,
                    Customername = c.Customername,
                    Customeremail = c.Customeremail,
                    Customerphone = c.Customerphone,
                    Totalpersons = o.Totalpersons,
                }
            ).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<CustomersViewModel>> GetAllCustomersOrderDetails()
        {
            var result = await (
                from mapping in _context.OrdersCustomersMappings
                join c in _context.Customers on mapping.Customerid equals c.Customerid
                join o in _context.Orders on mapping.Orderid equals o.Orderid into ordersGroup
                from o in ordersGroup.DefaultIfEmpty()
                where c.Isdeleted == false
                where o == null || c.Isdeleted == false
                select new CustomersViewModel
                {
                    Customerid = c.Customerid,
                    Customername = c.Customername,
                }
            ).ToListAsync();
            return result;
        }

        // order app

        public async Task<List<OrderItemModifierJoinModelView>> GetAllOrderItemModifierJoin()
        {
            var result = await (
                from oi in _context.OrderItemMappings
                join oim in _context.OrderItemModifiersMappings
                    on oi.Orderitemmappingid equals oim.Orderitemmappingid
                select new OrderItemModifierJoinModelView
                {
                    OrderItemMappingId = oi.Orderitemmappingid,
                    orderId = oi.Orderid,
                    itemId = oi.Itemid,
                    modifierId = oim.Modifierid,
                }
            ).ToListAsync();
            return result;
        }

        public async Task<List<OrderItemModifiersMappingViewModel>> GetOIMByOrderId(int orderid)
        {
            var result = await (
                from oi in _context.OrderItemMappings
                join oim in _context.OrderItemModifiersMappings
                on oi.Orderitemmappingid equals oim.Orderitemmappingid
                where oi.Orderid == orderid
                select new OrderItemModifiersMappingViewModel
                {
                    Mappingid = oim.Mappingid,
                    Orderitemmappingid = oi.Orderitemmappingid,
                    orderId = oi.Orderid,
                    itemId = oi.Itemid,
                    Modifierid = oim.Modifierid,
                    status = oi.Status ?? 0,
                    totalQuantity = oi.Totalquantity ?? 0,
                    ReadyQuantity = oi.Readyquantity ?? 0

                }
            ).ToListAsync();
            return result;
        }

        public async Task<List<OrdersTablesMapping>> GetTableByORderId(int orderid)
        {
            return await _context.OrdersTablesMappings.Where(o => o.Orderid == orderid).ToListAsync();
        }


        public async Task<List<Customer>> GetCustomerByEmail(string email)
        {
            return await _context
                .Customers.Where(c =>
                    c.Customeremail.ToLower().Contains(email) && c.Isdeleted == false
                )
                .ToListAsync();
        }


        public async Task<OrdersTablesMapping> GetOrderByTableId(int tableid)
        {
            var result = await (
                from ot in _context.OrdersTablesMappings
                join o in _context.Orders on ot.Orderid equals o.Orderid
                where ot.Tableid == tableid && o.Isdeleted == false
                orderby o.Createdat descending
                select new OrdersTablesMapping
                {
                    Orderid = o.Orderid,
                    Tableid = ot.Tableid
                }
            ).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<TaxAmountViewModel>> GetTaxByOrderId(int OrderId)
        {
            var result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId 
                select new TaxAmountViewModel
                {
                    TaxId = ot.Taxid,
                    TaxAmount = ot.Totalamount
                }
            ).ToListAsync();

            return result;
        }


        public async Task<List<OrderItemMapping>> GetOrderItemMappingByOrderId(int OrderId)
        {
            var result = await (
                from oi in _context.OrderItemMappings
                where oi.Orderid == OrderId
                select new OrderItemMapping
                {
                    Orderitemmappingid = oi.Orderitemmappingid,
                    Itemid = oi.Itemid,
                    Orderid = oi.Orderid,
                    Totalquantity = oi.Totalquantity,
                    Readyquantity = oi.Readyquantity,
                    Status = oi.Status,
                    Specialmessage = oi.Specialmessage,
                    Createdat = oi.Createdat
                }
            ).ToListAsync();

            return result;
        }

        public async Task<List<OrderTaxMapping>> GetOrderTaxMappingByOrderId(int OrderId)
        {
            var result = await (
                from ot in _context.OrderTaxMappings
                where ot.Orderid == OrderId
                select new OrderTaxMapping
                {
                    Ordertaxmappingid = ot.Ordertaxmappingid,
                    Taxid = ot.Taxid,
                    Orderid = ot.Orderid,
                    Totalamount = ot.Totalamount,
                    Createdat = ot.Createdat,
                    Editedat = ot.Editedat,
                    Createdbyid = ot.Createdbyid,
                }
            ).ToListAsync();

            return result;
        }

        public async Task<List<OrderItemModifiersMapping>> GetOIMByOrderItemMappingId(int Orderitemmappingid)
        {
            return await _context.OrderItemModifiersMappings.Where(o => o.Orderitemmappingid == Orderitemmappingid).ToListAsync();
        }

        public async Task<List<OrderTax>> GetTaxesByOrderId(int orderId){
            var result = await (
                from ot in _context.OrderTaxMappings
                join t in _context.TaxAndFees on ot.Taxid equals t.Taxid
                where ot.Orderid == orderId
                select new OrderTax
                {
                    TaxId = t.Taxid,
                    TaxName = t.Taxname,
                    TaxType = t.Taxtype,
                    TaxAmount = t.Taxamount,
                    Isenabled = t.Isenabled
                }
            ).ToListAsync();

            return result;
        }




        public async Task<List<TaxAndFee>?> CheckTaxByName(string name)
        {
            return await _context.TaxAndFees.Where(f => f.Taxname.ToLower().Equals(name) && f.Isdeleted == false).ToListAsync();
            
        }
        public async Task<List<Section>?> CheckSectionByName(string name)
        {
           return await _context.Sections.Where(s => s.Sectionname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }
        public async Task<List<Table>?> CheckTableByName(string name)
        {
           return await _context.Tables.Where(s => s.Tablename != null && s.Tablename.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }
        public async Task<List<Category>?> CheckCategoryByName(string name)
        {
           return await _context.Categories.Where(s => s.Categoryname != null && s.Categoryname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }
        public async Task<List<Modifiergroup>?> CheckModifierGroupByName(string name)
        {
           return await _context.Modifiergroups.Where(s => s.Modifiergroupname != null && s.Modifiergroupname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }

        public async Task<List<Item>?> CheckItemByName(string name)
        {
           return await _context.Items.Where(s => s.Itemname != null && s.Itemname.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }

        public async Task<List<Modifier>?> CheckModifierByName(string name)
        {
           return await _context.Modifiers.Where(s => s.Modifiername != null && s.Modifiername.Trim().ToLower().Equals(name) && s.Isdeleted == false).ToListAsync();
        }

        public async Task<Customer?> CheckCustomerByEmail(string email)
        {
           return await _context.Customers.Where(s => s.Customeremail != null && s.Customeremail.Trim().ToLower().Equals(email) && s.Isdeleted == false).FirstOrDefaultAsync();
        }



        // dashboard

        public async Task<List<ItemWithCount>> GetTopItemsOrderedAsync(int range, DateTime startDate, DateTime endDate)
        {
            var result = await (
                from oi in _context.OrderItemMappings
                join o in _context.Orders on oi.Orderid equals o.Orderid
                join i in _context.Items on oi.Itemid equals i.Itemid
                where o.Createdat >= startDate && o.Createdat <= endDate
                group new { oi, i } by new { i.Itemname, i.Imageid } into g
                orderby g.Count() descending
                select new ItemWithCount
                {
                    ItemName = g.Key.Itemname,
                    Count = g.Count(),
                    Image = g.Key.Imageid
                }
            ).Take(range).ToListAsync();

            return result;
        }

        public async Task<List<ItemWithCount>> GetLastItemsOrderedAsync(int range, DateTime startDate, DateTime endDate)
        {
            var result = await (
                from oi in _context.OrderItemMappings
                join o in _context.Orders on oi.Orderid equals o.Orderid
                join i in _context.Items on oi.Itemid equals i.Itemid
                where o.Createdat >= startDate && o.Createdat <= endDate
                group new { oi, i } by new { i.Itemname, i.Imageid } into g
                orderby g.Count() ascending
                select new ItemWithCount
                {
                    ItemName = g.Key.Itemname,
                    Count = g.Count(),
                    Image = g.Key.Imageid
                }
            ).Take(range).ToListAsync();

            return result;
        }

        public async Task<List<GraphRevenueViewModel>> GetGraphDataAsync(DateTime startDate, DateTime endDate)
        {
            var result = await (
                from o in _context.Orders
                where o.Isdeleted == false && o.Createdat >= startDate && o.Createdat <= endDate && o.Status == 5
                group o by o.Createdat.Value.Date into g
                orderby g.Key ascending
                select new GraphRevenueViewModel
                {
                    revenue = g.Sum(x => x.Totalamount),
                    date = g.Key,
                    dateNumber = g.Key.ToString("MMM dd")
                }
            ).ToListAsync();

            return result;
        }

        public async Task<List<GraphCustomerViewModel>> GetCustomerGraphDataAsync(DateTime startDate, DateTime endDate)
        {
            var result = await (
                from o in _context.Customers
                where o.Isdeleted == false && o.Createdat >= startDate && o.Createdat <= endDate 
                group o by o.Createdat.Value.Date into g
                orderby g.Key ascending
                select new GraphCustomerViewModel
                {
                    NumberOfCustomer = g.Select(x => x.Customerid).Distinct().Count(),
                    Date = g.Key,
                    Month = g.Key.ToString("MMM dd")
                }
            ).ToListAsync();

            return result;
        }


        public async Task<Feedback?> GetFeedbackByOrderId(int orderid)
        {
            var result = await _context.Feedbacks
                .Where(f => f.Orderid == orderid)
                .FirstOrDefaultAsync();

            return result;
        }


        public async Task<PasswordResetRequest?> GetTokenData(string token)
        {
            var result = await _context.PasswordResetRequests
                .Where(f => f.Guidtoken == token)
                .FirstOrDefaultAsync();

            return result;
        }

    }
}
