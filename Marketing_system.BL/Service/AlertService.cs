using Marketing_system.BL.Contracts.IService;
using Marketing_system.BL.Hubs;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Serilog;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Marketing_system.BL.Service
{
    public class AlertService : IAlertService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailHandler _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private string? accountSid;
        private string? authToken;

        public AlertService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext, IEmailHandler emailService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _emailService = emailService;
            accountSid = configuration["TwilioConfig:AccountSid"];
            authToken = configuration["TwilioConfig:AuthToken"];
        }

        public async Task AlertAsync(string message)
        {
            TwilioClient.Init(accountSid, authToken);
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);

            var users = await _unitOfWork.GetUserRepository().GetAdmins();
            foreach (var user in users)
            {
                await _emailService.SendEmail(user.Email, message, "<p>Please check your app, because a fatal error has occurred</p>");
            }

            var messageOptions = new CreateMessageOptions(new PhoneNumber("+381649460263"))
            {
                From = new PhoneNumber("+15513776301"),
                Body = message
            };
            var sms = MessageResource.Create(messageOptions);
            Log.Information(sms.Body);
        }
    }
}
