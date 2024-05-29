using Marketing_system.BL.Contracts.IService;

namespace Marketing_system.BL.Service
{
    public class TempTokenManagerService : ITempTokenManagerService
    {
        private readonly Dictionary<string, string> _temporaryTokens = new();

        public void AddToken(string token, string username)
        {
            if (_temporaryTokens.ContainsValue(username))
            {
                RemoveToken(token);
            }
            _temporaryTokens[token] = username;
        }

        public void RemoveToken(string token)
        {
            _temporaryTokens.Remove(token);
        }

        public bool TryGetUsername(string token, out string? username)
        {
            return _temporaryTokens.TryGetValue(token, out username);
        }
    }
}
