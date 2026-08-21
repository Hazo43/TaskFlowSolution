using Domain.Entites;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ISpecifications<TEntity , TKey> where TEntity : BaseEntity<TKey>
    {
        // where
        public Expression<Func<TEntity , bool>> Criteria { get; }

        // Include ( Join )
        public List<Expression<Func<TEntity , object>>> Includes { get; }

        // OrderBy
        public Expression<Func<TEntity , object>>? OrderBy { get; }
        
        // OrderByDesc
        public Expression<Func<TEntity , object>>? OrderByDesc { get; }

        // Pagination ( Take , Skip ) 
        public int Skip { get; }
        public int Take { get; }
        public bool IsPaginated { get; }


    }
}
