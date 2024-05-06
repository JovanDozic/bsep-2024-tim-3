using Marketing_system.DA.Contracts.IRepository;
using Marketing_system.DA.Contracts.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Marketing_system.DA.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        public readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<EntityEntry<TEntity>> Add(TEntity entity)
        {
            return await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        }
        public TEntity Update(TEntity entity)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entity);
            }
            catch (DbUpdateException e)
            {
                throw new KeyNotFoundException(e.Message);
            }
            return entity;
        }

        public void Delete(int id)
        {
            var entity = GetByIdAsync(id);
            _dbContext.Set<TEntity>().Remove(entity.Result);
        }
    }
}
