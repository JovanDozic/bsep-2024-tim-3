using Marketing_system.BL.Contracts.IService;

namespace Marketing_system.BL.Service
{
    public class TempTokenManagerService : ITempTokenManagerService
    {
        private readonly Dictionary<string, string> _temporaryTokens = new();

        public void AddToken(string token, string email)
        {
            if (_temporaryTokens.ContainsValue(email))
            {
                RemoveToken(token);
            }
            _temporaryTokens[token] = email;
        }

        public void RemoveToken(string token)
        {
            _temporaryTokens.Remove(token);
        }

        public bool TryGetEmail(string token, out string? email)
        {
            return _temporaryTokens.TryGetValue(token, out email);
        }

        public void RemoveTokenByEmail(string email)
        {
            _temporaryTokens.Remove(_temporaryTokens.FirstOrDefault(x => x.Value == email).Key);
        }
    }
}
