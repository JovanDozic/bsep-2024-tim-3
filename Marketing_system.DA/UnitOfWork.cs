using Marketing_system.DA.Contexts;
using Marketing_system.DA.Contracts;

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
            } catch(Exception ex)
            {
                Console.WriteLine($"Error savig changes to the database: {ex.Message}");
                throw;
            }
        }
    }
}
