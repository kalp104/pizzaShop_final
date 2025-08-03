using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Repository.Interfaces;

public interface IWaitingListRepository
{
    Task<List<WaitingList>?> GetWaitingListsByDateFilters(DateTime startDate, DateTime endDate);
    Task<string> AddWaitingList(WaitingList waitingList);
    Task<List<WaitingList>?> GetAllWaitingLists();
    Task<List<WaitingList>?> GetAllWaitingListsBySectionId(int? sectionid = null);

    Task<WaitingList?> GetWaitingListById(int waitinglist);
    Task<string> UpdateWaitingList(WaitingList waitingList);




}
