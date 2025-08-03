using System;
using System.Collections.Generic;

namespace PizzaShop.Repository.Models;

public partial class Feedback
{
    public int Feedbackid { get; set; }

    public int Orderid { get; set; }

    public int? FoodRating { get; set; }

    public int? ServiceRating { get; set; }

    public int? AmbienceRating { get; set; }

    public string? CommentsFeedback { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Editedat { get; set; }

    public DateTime? Deletedat { get; set; }

    public int? Createdbyid { get; set; }

    public int? Editedbyid { get; set; }

    public int? Deletedbyid { get; set; }

    public virtual Order Order { get; set; } = null!;
}
