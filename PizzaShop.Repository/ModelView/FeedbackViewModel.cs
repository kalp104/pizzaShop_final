using System;

namespace PizzaShop.Repository.ModelView;

public class FeedbackViewModel
{
    public int foodRating {get;set;}
    public int serviceRating {get;set;}
    public int ambienceRating {get;set;}
    public string? comment {get;set;}

}
