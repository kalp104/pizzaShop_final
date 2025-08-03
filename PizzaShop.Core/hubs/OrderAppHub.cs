using Microsoft.AspNetCore.SignalR;

namespace PizzaShop.Core.hubs;

public class OrderAppHub : Hub
{
    public async Task SendOrderApp()
    {
        await Clients.All.SendAsync("ReceiveOrderApp");
    }
}
