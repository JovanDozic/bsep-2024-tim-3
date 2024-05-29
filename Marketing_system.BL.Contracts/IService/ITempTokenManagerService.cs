namespace Marketing_system.BL.Contracts.IService
{
    public interface ITempTokenManagerService
    {
        void AddToken(string token, string username);
        bool TryGetUsername(string token, out string? username);
        void RemoveToken(string token);
    }
}
