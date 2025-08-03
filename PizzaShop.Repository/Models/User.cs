
using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class User
{
    public int Userid { get; set; }

    public int Roleid { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public decimal? Phone { get; set; }

    public int Countryid { get; set; }

    public string? Address { get; set; }

    public decimal? Zipcode { get; set; }

    public bool Isactive { get; set; }

    public string? Userimage { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public DateTime? Editedat { get; set; }

    public int Accountid { get; set; }

    public bool? Isdeleted { get; set; }

    public int Cityid { get; set; }

    public int Stateid { get; set; }

    public int? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual City? City { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Role Role { get; set; } = null!;

    public virtual State? State { get; set; }
}
