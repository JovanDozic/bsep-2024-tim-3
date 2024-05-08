namespace Marketing_system.DA.Contracts.IRepository
{
    public interface IEmailHandler
    {
        Task<bool> SendPasswordlessLink(string email, string link);
    }
}
