using Microsoft.AspNetCore.SignalR;
using Marketing_system.BL.Contracts.IService;

namespace Marketing_system.BL.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IAlertService _alertService;

        public NotificationHub(IAlertService alertService)
        {
            _alertService = alertService;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", user, message);
        }
    }
}
