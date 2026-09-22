using Domain.Entites;

namespace Domain.Interfaces
{
    public interface IGenericRepository<TEntity , TKey> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);

        #region Specifications

        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications);

        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications);

        #endregion

    }
}
