using System;

namespace PizzaShop.Repository.ModelView;

public class UserTableViewModel
{
    public int Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public int? Role { get; set; }
    public Decimal? Phone { get; set; }
    public int? Status { get; set; }
    public string? Image { get; set; }
    public string? Rolename { get; set; }
    public int AccountId { get; set; }
}
