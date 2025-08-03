using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface IFeedBackRepository
{
    Task AddFeedback(Feedback feedback);
}
