namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IEmailHandler
    {
        Task<bool> SendLinkToEmail(string email, string link, string subject);
    }
}
