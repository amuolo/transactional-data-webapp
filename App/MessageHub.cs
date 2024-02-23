using App.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace App;

public class MessageHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync(Consts.ReceiveMessage, user, message);
    }
}
