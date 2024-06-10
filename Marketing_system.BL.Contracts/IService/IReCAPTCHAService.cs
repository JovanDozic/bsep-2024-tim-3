namespace Marketing_system.BL.Contracts.IService
{
    public interface IReCAPTCHAService
    {
        Task<bool> VerifyToken(string token);
    }
}
