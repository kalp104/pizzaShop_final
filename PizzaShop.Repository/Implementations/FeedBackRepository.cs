using System;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Implementations;

public class FeedBackRepository : IFeedBackRepository
{
    private readonly PizzaShop2Context _context;

    public FeedBackRepository(PizzaShop2Context context)
    {
        _context = context;       
    }

    public async Task AddFeedback(Feedback feedback)
    {   
        _context.Add(feedback);
        await _context.SaveChangesAsync();
    } 

}
