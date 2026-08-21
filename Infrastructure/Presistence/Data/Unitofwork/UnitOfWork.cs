using Domain.Entites;
using Domain.Interfaces;
using Presistence.Data.DbContexts;
using Presistence.Data.Repositories;

namespace Presistence.Data.Unitofwork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskFlowDbContext _dbContext;

        // key (string) and value (object) بيخزن البيانات 
        // 1- key like (project , task , Comment )
        // 2- value like ( GetRepository<Project, int>() )
        private readonly Dictionary<string, object> _repository = [];
            public UnitOfWork(TaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            // "Tasks" , "Project" اللي هيه زي ال TEntity  بنجيب ال
            var entityType = typeof(TEntity).Name;

            // entityType = Projects , Tasks



            if (!_repository.ContainsKey(entityType))  // Objects , Tasks لو مش بيحتوي ع
                                                       // _repository
                                                       // "Project" → GenericRepository<Project, int>
                _repository[entityType] = new GenericRepository<TEntity, TKey>(_dbContext); // روح اعملو واحد جديد

            // رجعهالو Projects او Tasks لاكن لو فيه
            return (IGenericRepository<TEntity, TKey>)_repository[entityType];
        }

        public async Task<int> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();
    }
}
