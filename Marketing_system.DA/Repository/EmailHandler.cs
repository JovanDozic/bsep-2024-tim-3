using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Model;
using Microsoft.Extensions.Options;
using System.Net.Mail;
namespace Marketing_system.DA.Repository
{
    public class EmailHandler : IEmailHandler
    {
        private readonly SMTPConfig _smtpConfig;
        public EmailHandler(IOptions<SMTPConfig> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        public async Task<bool> SendEmail(string email, string link, string subject)
        {
            var from = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName);
            var to = new MailAddress(email);

            var smtp = new SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                Credentials = new System.Net.NetworkCredential(from.Address, _smtpConfig.Password)
            };

            using var message = new MailMessage(from, to)
            {
                Subject = subject,
                Body = link,
                IsBodyHtml = true
            };

            try
            {
                await smtp.SendMailAsync(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
