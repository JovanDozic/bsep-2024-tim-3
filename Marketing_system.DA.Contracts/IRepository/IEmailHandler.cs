namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IEmailHandler
    {
        Task<bool> SendEmail(string email, string body, string subject);
    }
}
