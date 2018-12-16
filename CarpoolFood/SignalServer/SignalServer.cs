using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CarpoolFood.SignalServer
{
    public class SignalRServer: Hub
    {
        public async Task SendNotification(int userId, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", userId, message);
        }
    }
}
