using Microsoft.AspNetCore.SignalR;
using Marketing_system.BL.Contracts.IService;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.BL.Hubs;

namespace Marketing_system.BL.Service
{
    public class AlertService : IAlertService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailHandler _emailService;

        public AlertService(IHubContext<NotificationHub> hubContext, IEmailHandler emailService)
        {
            _hubContext = hubContext;
            _emailService = emailService;
        }

        public async Task AlertAsync(string message)
        {
            // Send real-time notification
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            // Send email notification
            await _emailService.SendEmail("admin@example.com", "Critical Alert", message);
        }
    }
}
