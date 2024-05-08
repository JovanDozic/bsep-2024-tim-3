using Marketing_system.DA.Contracts.IRepository;
using System.Net.Mail;
namespace Marketing_system.DA.Repository
{
    public class EmailHandler : IEmailHandler
    {
        public async Task<bool> SendPasswordlessLink(string email, string link)
        {
            var from = new MailAddress("covid23serbia@gmail.com", "BSEP Marketing app");
            var to = new MailAddress(email);
            const string fromPassword = "pzty nfyd fyfn trqt"; // TODO: Load this from appsettings.json

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(from.Address, fromPassword)
            };

            using var message = new MailMessage(from, to)
            {
                Subject = "Passwordless login link",
                Body = link
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
