using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Models;

public partial class PizzaShop2Context : DbContext
{
    public PizzaShop2Context()
    {
    }

    public PizzaShop2Context(DbContextOptions<PizzaShop2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventCustomerMapping> EventCustomerMappings { get; set; }

    public virtual DbSet<EventSpecialType> EventSpecialTypes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemModifiergroupMapping> ItemModifiergroupMappings { get; set; }

    public virtual DbSet<Modfierandgroupsmapping> Modfierandgroupsmappings { get; set; }

    public virtual DbSet<Modifier> Modifiers { get; set; }

    public virtual DbSet<Modifiergroup> Modifiergroups { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItemMapping> OrderItemMappings { get; set; }

    public virtual DbSet<OrderItemModifiersMapping> OrderItemModifiersMappings { get; set; }

    public virtual DbSet<OrderTaxMapping> OrderTaxMappings { get; set; }

    public virtual DbSet<OrdersCustomersMapping> OrdersCustomersMappings { get; set; }

    public virtual DbSet<OrdersTablesMapping> OrdersTablesMappings { get; set; }

    public virtual DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionsRole> PermissionsRoles { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TaxAndFee> TaxAndFees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<User2faAuth> User2faAuths { get; set; }

    public virtual DbSet<WaitingList> WaitingLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=pizzaShop2;Username=postgres;password=tatva123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Accountid).HasName("account_pkey");

            entity.ToTable("account");

            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Rememberme)
                .HasDefaultValue(false)
                .HasColumnName("rememberme");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("fk_roleid");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Categorydescription)
                .HasMaxLength(255)
                .HasColumnName("categorydescription");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(50)
                .HasColumnName("categoryname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Cityid).HasName("city_pkey");

            entity.ToTable("city");

            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Cityname)
                .HasMaxLength(50)
                .HasColumnName("cityname");
            entity.Property(e => e.Stateid).HasColumnName("stateid");

            entity.HasOne(d => d.State).WithMany(p => p.Cities)
                .HasForeignKey(d => d.Stateid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("city_stateid_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Countryid).HasName("country_pkey");

            entity.ToTable("country");

            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Countryname)
                .HasMaxLength(50)
                .HasColumnName("countryname");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("customers_pkey");

            entity.ToTable("customers");

            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Customeremail)
                .HasMaxLength(100)
                .HasColumnName("customeremail");
            entity.Property(e => e.Customername)
                .HasMaxLength(100)
                .HasColumnName("customername");
            entity.Property(e => e.Customerphone).HasColumnName("customerphone");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("events_pkey");

            entity.ToTable("events");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.AcSwitch).HasColumnName("ac_switch");
            entity.Property(e => e.BillingAddress)
                .HasColumnType("character varying")
                .HasColumnName("billing_address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedById)
                .HasDefaultValue(0)
                .HasColumnName("created_by_id");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DeletedById)
                .HasDefaultValue(0)
                .HasColumnName("deleted_by_id");
            entity.Property(e => e.EditedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("edited_at");
            entity.Property(e => e.EditedById)
                .HasDefaultValue(0)
                .HasColumnName("edited_by_id");
            entity.Property(e => e.EndEventTime).HasColumnName("End_Event_Time");
            entity.Property(e => e.EventDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("event_date");
            entity.Property(e => e.EventStatus).HasColumnName("event_status");
            entity.Property(e => e.EventType).HasColumnName("event_type");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.NumberOfPerson).HasColumnName("number_of_person");
            entity.Property(e => e.OrderType).HasColumnName("order_type");
            entity.Property(e => e.SpecialInstruction)
                .HasColumnType("character varying")
                .HasColumnName("special_instruction");
            entity.Property(e => e.StartEventTime).HasColumnName("Start_Event_Time");
        });

        modelBuilder.Entity<EventCustomerMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId).HasName("event_customer_mapping_pkey");

            entity.ToTable("event_customer_mapping");

            entity.Property(e => e.MappingId).HasColumnName("mapping_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedById)
                .HasDefaultValue(0)
                .HasColumnName("created_by_id");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.EditedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("edited_at");
            entity.Property(e => e.EditedById)
                .HasDefaultValue(0)
                .HasColumnName("edited_by_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.EventCustomerMappings)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_customer_mapping_customerid_fkey");

            entity.HasOne(d => d.Event).WithMany(p => p.EventCustomerMappings)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_customer_mapping_event_id_fkey");
        });

        modelBuilder.Entity<EventSpecialType>(entity =>
        {
            entity.HasKey(e => e.SpecialTypeId).HasName("event_special_type_pkey");

            entity.ToTable("event_special_type");

            entity.Property(e => e.SpecialTypeId).HasColumnName("special_type_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EventTypeDetails)
                .HasColumnType("character varying")
                .HasColumnName("event_type_details");

            entity.HasOne(d => d.Event).WithMany(p => p.EventSpecialTypes)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_special_type_event_id_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Feedbackid).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.Feedbackid).HasColumnName("feedbackid");
            entity.Property(e => e.AmbienceRating)
                .HasDefaultValue(0)
                .HasColumnName("ambience_rating");
            entity.Property(e => e.CommentsFeedback)
                .HasMaxLength(255)
                .HasColumnName("comments_feedback");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.FoodRating)
                .HasDefaultValue(0)
                .HasColumnName("food_rating");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.ServiceRating)
                .HasDefaultValue(0)
                .HasColumnName("service_rating");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_orderid_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("item_pkey");

            entity.ToTable("item");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.DefaultTax)
                .HasDefaultValue(false)
                .HasColumnName("defaultTax");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Favourite).HasColumnName("favourite");
            entity.Property(e => e.Imageid)
                .HasMaxLength(500)
                .HasColumnName("imageid");
            entity.Property(e => e.Isavailable)
                .HasDefaultValue(false)
                .HasColumnName("isavailable");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Itemname)
                .HasMaxLength(100)
                .HasColumnName("itemname");
            entity.Property(e => e.Itemtype).HasColumnName("itemtype");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Shortcode).HasColumnName("shortcode");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Taxpercentage).HasColumnName("taxpercentage");
            entity.Property(e => e.Units).HasColumnName("units");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_categoryid_fkey");
        });

        modelBuilder.Entity<ItemModifiergroupMapping>(entity =>
        {
            entity.HasKey(e => e.Itemmodifierid).HasName("item_modifiergroup_mapping_pkey");

            entity.ToTable("item_modifiergroup_mapping");

            entity.Property(e => e.Itemmodifierid).HasColumnName("itemmodifierid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Maxvalue)
                .HasDefaultValue(0)
                .HasColumnName("maxvalue");
            entity.Property(e => e.Minvalue)
                .HasDefaultValue(0)
                .HasColumnName("minvalue");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroup_mapping_itemid_fkey");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.ItemModifiergroupMappings)
                .HasForeignKey(d => d.Modifiergroupid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("item_modifiergroup_mapping_modifiergroupid_fkey");
        });

        modelBuilder.Entity<Modfierandgroupsmapping>(entity =>
        {
            entity.HasKey(e => e.Modfierandgroupsmappingid).HasName("modfierandgroupsmapping_pkey");

            entity.ToTable("modfierandgroupsmapping");

            entity.Property(e => e.Modfierandgroupsmappingid).HasColumnName("modfierandgroupsmappingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdelete)
                .HasDefaultValue(false)
                .HasColumnName("isdelete");
            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Modifierid).HasColumnName("modifierid");

            entity.HasOne(d => d.Modifiergroup).WithMany(p => p.Modfierandgroupsmappings)
                .HasForeignKey(d => d.Modifiergroupid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modfierandgroupsmapping_modifiergroupid_fkey");

            entity.HasOne(d => d.Modifier).WithMany(p => p.Modfierandgroupsmappings)
                .HasForeignKey(d => d.Modifierid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modfierandgroupsmapping_modifierid_fkey");
        });

        modelBuilder.Entity<Modifier>(entity =>
        {
            entity.HasKey(e => e.Modifierid).HasName("modifiers_pkey");

            entity.ToTable("modifiers");

            entity.Property(e => e.Modifierid).HasColumnName("modifierid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifierdescription)
                .HasMaxLength(500)
                .HasColumnName("modifierdescription");
            entity.Property(e => e.Modifiername)
                .HasMaxLength(50)
                .HasColumnName("modifiername");
            entity.Property(e => e.Modifierquantity).HasColumnName("modifierquantity");
            entity.Property(e => e.Modifierrate).HasColumnName("modifierrate");
            entity.Property(e => e.Modifierunit).HasColumnName("modifierunit");
            entity.Property(e => e.Taxdefault)
                .HasDefaultValue(false)
                .HasColumnName("taxdefault");
            entity.Property(e => e.Taxpercentage).HasColumnName("taxpercentage");
        });

        modelBuilder.Entity<Modifiergroup>(entity =>
        {
            entity.HasKey(e => e.Modifiergroupid).HasName("modifiergroup_pkey");

            entity.ToTable("modifiergroup");

            entity.Property(e => e.Modifiergroupid).HasColumnName("modifiergroupid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Modifiergroupdescription)
                .HasMaxLength(50)
                .HasColumnName("modifiergroupdescription");
            entity.Property(e => e.Modifiergroupname)
                .HasMaxLength(50)
                .HasColumnName("modifiergroupname");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Completedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("completedat");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Orderdescription)
                .HasMaxLength(255)
                .HasColumnName("orderdescription");
            entity.Property(e => e.Ordermessage)
                .HasMaxLength(100)
                .HasColumnName("ordermessage");
            entity.Property(e => e.Ordertype)
                .HasDefaultValue(1)
                .HasColumnName("ordertype");
            entity.Property(e => e.Paymentmode).HasColumnName("paymentmode");
            entity.Property(e => e.Ratings)
                .HasDefaultValueSql("0")
                .HasColumnName("ratings");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Totalamount).HasColumnName("totalamount");
            entity.Property(e => e.Totalpersons)
                .HasDefaultValue(1)
                .HasColumnName("totalpersons");
        });

        modelBuilder.Entity<OrderItemMapping>(entity =>
        {
            entity.HasKey(e => e.Orderitemmappingid).HasName("order_item_mapping_pkey");

            entity.ToTable("order_item_mapping");

            entity.Property(e => e.Orderitemmappingid).HasColumnName("orderitemmappingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Readyquantity)
                .HasDefaultValue(0)
                .HasColumnName("readyquantity");
            entity.Property(e => e.Specialmessage)
                .HasMaxLength(100)
                .HasDefaultValueSql("'xyz'::character varying")
                .HasColumnName("specialmessage");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Totalquantity)
                .HasDefaultValueSql("0")
                .HasColumnName("totalquantity");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderItemMappings)
                .HasForeignKey(d => d.Itemid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_mapping_itemid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItemMappings)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_mapping_orderid_fkey");
        });

        modelBuilder.Entity<OrderItemModifiersMapping>(entity =>
        {
            entity.HasKey(e => e.Mappingid).HasName("order_item_modifiers_mapping_pkey");

            entity.ToTable("order_item_modifiers_mapping");

            entity.Property(e => e.Mappingid).HasColumnName("mappingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Modifierid).HasColumnName("modifierid");
            entity.Property(e => e.Orderitemmappingid).HasColumnName("orderitemmappingid");

            entity.HasOne(d => d.Modifier).WithMany(p => p.OrderItemModifiersMappings)
                .HasForeignKey(d => d.Modifierid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_modifiers_mapping_modifierid_fkey");

            entity.HasOne(d => d.Orderitemmapping).WithMany(p => p.OrderItemModifiersMappings)
                .HasForeignKey(d => d.Orderitemmappingid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_modifiers_mapping_orderitemmappingid_fkey");
        });

        modelBuilder.Entity<OrderTaxMapping>(entity =>
        {
            entity.HasKey(e => e.Ordertaxmappingid).HasName("order_tax_mapping_pkey");

            entity.ToTable("order_tax_mapping");

            entity.Property(e => e.Ordertaxmappingid).HasColumnName("ordertaxmappingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Taxid).HasColumnName("taxid");
            entity.Property(e => e.Totalamount).HasColumnName("totalamount");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_orderid_fkey");

            entity.HasOne(d => d.Tax).WithMany(p => p.OrderTaxMappings)
                .HasForeignKey(d => d.Taxid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_tax_mapping_taxid_fkey");
        });

        modelBuilder.Entity<OrdersCustomersMapping>(entity =>
        {
            entity.HasKey(e => e.OrderCustomerMappingId).HasName("orders_customers_mapping_pkey");

            entity.ToTable("orders_customers_mapping");

            entity.Property(e => e.OrderCustomerMappingId).HasColumnName("order_customer_mapping_id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrdersCustomersMappings)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customers_mapping_customerid_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersCustomersMappings)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_customers_mapping_orderid_fkey");
        });

        modelBuilder.Entity<OrdersTablesMapping>(entity =>
        {
            entity.HasKey(e => e.Orderstableid).HasName("orders_tables_mapping_pkey");

            entity.ToTable("orders_tables_mapping");

            entity.Property(e => e.Orderstableid).HasColumnName("orderstableid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.Tableid).HasColumnName("tableid");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersTablesMappings)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_tables_mapping_orderid_fkey");

            entity.HasOne(d => d.Table).WithMany(p => p.OrdersTablesMappings)
                .HasForeignKey(d => d.Tableid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_tables_mapping_tableid_fkey");
        });

        modelBuilder.Entity<PasswordResetRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("password_reset_requests_pkey");

            entity.ToTable("password_reset_requests");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Closedate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("closedate");
            entity.Property(e => e.Createdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdate");
            entity.Property(e => e.Guidtoken)
                .HasMaxLength(255)
                .HasColumnName("guidtoken");
            entity.Property(e => e.Userid).HasColumnName("userid");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Permissionid).HasName("permissions_pkey");

            entity.ToTable("permissions");

            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Permissionname)
                .HasMaxLength(50)
                .HasColumnName("permissionname");
        });

        modelBuilder.Entity<PermissionsRole>(entity =>
        {
            entity.HasKey(e => e.Permissionroleid).HasName("permissions_role_pkey");

            entity.ToTable("permissions_role");

            entity.Property(e => e.Permissionroleid).HasColumnName("permissionroleid");
            entity.Property(e => e.Candelete).HasColumnName("candelete");
            entity.Property(e => e.Canedit).HasColumnName("canedit");
            entity.Property(e => e.Canview).HasColumnName("canview");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid).HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");
            entity.Property(e => e.Permissionid).HasColumnName("permissionid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Permission).WithMany(p => p.PermissionsRoles)
                .HasForeignKey(d => d.Permissionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_permissionid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.PermissionsRoles)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("permissions_role_roleid_fkey");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtokens_pkey");

            entity.ToTable("refreshtokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Created)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created");
            entity.Property(e => e.Expires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires");
            entity.Property(e => e.Isrevoked)
                .HasDefaultValue(false)
                .HasColumnName("isrevoked");
            entity.Property(e => e.Token)
                .HasMaxLength(512)
                .HasColumnName("token");
            entity.Property(e => e.Useremail)
                .HasMaxLength(255)
                .HasColumnName("useremail");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.Sectionid).HasName("sections_pkey");

            entity.ToTable("sections");

            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Sectiondescription)
                .HasMaxLength(50)
                .HasColumnName("sectiondescription");
            entity.Property(e => e.Sectionname)
                .HasMaxLength(50)
                .HasColumnName("sectionname");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Stateid).HasName("state_pkey");

            entity.ToTable("state");

            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Statename)
                .HasMaxLength(50)
                .HasColumnName("statename");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("state_countryid_fkey");
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Tableid).HasName("tables_pkey");

            entity.ToTable("tables");

            entity.Property(e => e.Tableid).HasColumnName("tableid");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Tablename)
                .HasMaxLength(50)
                .HasColumnName("tablename");

            entity.HasOne(d => d.Section).WithMany(p => p.Tables)
                .HasForeignKey(d => d.Sectionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tables_sectionid_fkey");
        });

        modelBuilder.Entity<TaxAndFee>(entity =>
        {
            entity.HasKey(e => e.Taxid).HasName("tax_and_fees_pkey");

            entity.ToTable("tax_and_fees");

            entity.Property(e => e.Taxid).HasColumnName("taxid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdefault)
                .HasDefaultValue(false)
                .HasColumnName("isdefault");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Isenabled)
                .HasDefaultValue(false)
                .HasColumnName("isenabled");
            entity.Property(e => e.Taxamount).HasColumnName("taxamount");
            entity.Property(e => e.Taxname)
                .HasMaxLength(100)
                .HasColumnName("taxname");
            entity.Property(e => e.Taxtype).HasColumnName("taxtype");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Cityid).HasColumnName("cityid");
            entity.Property(e => e.Countryid).HasColumnName("countryid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid).HasColumnName("createdbyid");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid).HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.Isactive).HasColumnName("isactive");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Phone)
                .HasPrecision(50)
                .HasColumnName("phone");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Stateid).HasColumnName("stateid");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Userimage)
                .HasMaxLength(500)
                .HasColumnName("userimage");
            entity.Property(e => e.Zipcode)
                .HasPrecision(20)
                .HasColumnName("zipcode");

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.Accountid)
                .HasConstraintName("fk_accountid");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.Cityid)
                .HasConstraintName("fk_cityid");

            entity.HasOne(d => d.Country).WithMany(p => p.Users)
                .HasForeignKey(d => d.Countryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_countryid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_roleid_fkey");

            entity.HasOne(d => d.State).WithMany(p => p.Users)
                .HasForeignKey(d => d.Stateid)
                .HasConstraintName("fk_stateid");
        });

        modelBuilder.Entity<User2faAuth>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("user_2fa_auth_pkey");

            entity.ToTable("user_2fa_auth");

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Counting)
                .HasDefaultValue(0)
                .HasColumnName("counting");
            entity.Property(e => e.CreateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_time");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.ExpireTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire_time");
            entity.Property(e => e.Rememberme).HasColumnName("rememberme");
            entity.Property(e => e.TokenAuth)
                .HasColumnType("character varying")
                .HasColumnName("token_auth");
        });

        modelBuilder.Entity<WaitingList>(entity =>
        {
            entity.HasKey(e => e.Waitingid).HasName("waiting_list_pkey");

            entity.ToTable("waiting_list");

            entity.Property(e => e.Waitingid).HasColumnName("waitingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdbyid)
                .HasDefaultValue(0)
                .HasColumnName("createdbyid");
            entity.Property(e => e.Customeremail)
                .HasMaxLength(100)
                .HasColumnName("customeremail");
            entity.Property(e => e.Customername)
                .HasMaxLength(200)
                .HasColumnName("customername");
            entity.Property(e => e.Customerphone).HasColumnName("customerphone");
            entity.Property(e => e.Deletedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deletedat");
            entity.Property(e => e.Deletedbyid)
                .HasDefaultValue(0)
                .HasColumnName("deletedbyid");
            entity.Property(e => e.Editedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("editedat");
            entity.Property(e => e.Editedbyid)
                .HasDefaultValue(0)
                .HasColumnName("editedbyid");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Sectionid).HasColumnName("sectionid");
            entity.Property(e => e.Totalperson)
                .HasDefaultValue(0)
                .HasColumnName("totalperson");

            entity.HasOne(d => d.Section).WithMany(p => p.WaitingLists)
                .HasForeignKey(d => d.Sectionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("waiting_list_sectionid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
