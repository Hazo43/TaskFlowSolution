using Domain.Entites;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Presistence.Data.DbContexts;

namespace Presistence.Data.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly TaskFlowDbContext _dbContext;

        public GenericRepository(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
                   => await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id)
                => await _dbContext.Set<TEntity>().FindAsync(id);

        public void Remove(TEntity entity)
               => _dbContext.Set<TEntity>().Remove(entity);

        public void Update(TEntity entity)
               => _dbContext.Set<TEntity>().Update(entity);

        #region Specification

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationsEvaluator.CreateQuery(_dbContext.Set<TEntity>(), specifications).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationsEvaluator.CreateQuery(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();
        }

        #endregion

    }
}
