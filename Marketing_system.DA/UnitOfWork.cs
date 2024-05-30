using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts;
using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Repository;

namespace Marketing_system.DA
{
    public class UnitOfWork : IUnitOfWork
    {
        private DataContext _context;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        public async void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            _context = null;
        }

        public async Task<int> Save()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error savig changes to the database: {ex.Message}");
                throw;
            }
        }

        private IUserRepository _userRepository { get; set; }
        private ITokenGeneratorRepository _tokenGeneratorRepository { get; set; }
        private IPasswordHasher _passwordHasher { get; set; }
        private IRegistrationRequestRepository _registrationRequestRepository { get; set; }
        private IPasswordlessTokenRepository _passwordlessTokenRepository { get; set; }
        private IAdvertisementRepository _advertisementRepository { get; set; }
        private IRoleRepository _roleRepository { get; set; }
        private IPasswordResetTokenRepository _passwordResetTokenRepository { get; set; }

        public IUserRepository GetUserRepository()
        {
            return _userRepository ?? (_userRepository = new UserRepository(_context));
        }

        public ITokenGeneratorRepository GetTokenGeneratorRepository()
        {
            return _tokenGeneratorRepository ?? (_tokenGeneratorRepository = new TokenGeneratorRepository());
        }
        public IPasswordHasher GetPasswordHasher()
        {
            return _passwordHasher ?? (_passwordHasher = new PasswordHasher());
        }
        public IRegistrationRequestRepository GetRegistrationRequestRepository()
        {
            return _registrationRequestRepository ?? (_registrationRequestRepository = new RegistrationRequestRepository(_context));
        }
        public IPasswordlessTokenRepository GetPasswordlessTokenRepository()
        {
            return _passwordlessTokenRepository ?? (_passwordlessTokenRepository = new PasswordlessTokenRepository(_context));
        }
        public IAdvertisementRepository GetAdvertisementRepository()
        {
            return _advertisementRepository ?? (_advertisementRepository = new AdvertisementRepository(_context));
        }
        public IRoleRepository GetRoleRepository()
        {
            return _roleRepository ?? (_roleRepository = new RoleRepository(_context));
        }

        public IPasswordResetTokenRepository GetPasswordResetTokenRepository()
        {
            return _passwordResetTokenRepository ?? (_passwordResetTokenRepository = new PasswordResetTokenRepository(_context));
        }
    }
}
