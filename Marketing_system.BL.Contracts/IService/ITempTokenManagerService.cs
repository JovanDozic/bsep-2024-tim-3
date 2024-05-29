namespace Marketing_system.BL.Contracts.IService
{
    public interface ITempTokenManagerService
    {
        void AddToken(string token, string email);
        bool TryGetEmail(string token, out string? email);
        void RemoveToken(string token);
        public void RemoveTokenByEmail(string email);
    }
}
