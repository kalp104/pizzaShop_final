namespace PizzaShop.Repository.ModelView;

public class CustomersViewModel
{
    public int Customerid { get; set; }

    public string Customername { get; set; } = null!;

    public string Customeremail { get; set; } = null!;

    public decimal Customerphone { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public bool? Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public int totalOrders { get; set; }

    public decimal MaxOrders { get; set; }

    public decimal AvgBill { get; set; }
    

    public List<OrderDetialsViewModel>? OrderDetails { get; set; }

}


public class OrderDetialsViewModel
{
    public DateTime? OrderDate { get; set; }
    public int OrderType { get; set; }
    public int Paymentmode { get; set; }
    public decimal amount { get; set; }
    public int TotalOrderedITems { get; set; }
}