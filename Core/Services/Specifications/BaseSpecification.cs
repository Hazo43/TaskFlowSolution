using Domain.Entites;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Services.Specifications
{
    public class BaseSpecification<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {

        public BaseSpecification()
        {
            Criteria = x => true;
        }

        #region Where [ Criteria ]

        public Expression<Func<TEntity, bool>> Criteria { get; private set; }
        protected BaseSpecification(Expression<Func<TEntity, bool>> critiriaExpression)
        {
            Criteria = critiriaExpression;
        }

        #endregion

        #region Includes
        public List<Expression<Func<TEntity, object>>> Includes { get;} = new();

        protected void AddInclude(Expression<Func<TEntity, object>> IncludesExpression)
        {
            Includes.Add(IncludesExpression);
        }
        #endregion

        #region OrderBy 
        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
        protected void AddOrderBy(Expression<Func<TEntity, object>> OrderByExpression)
        {
            OrderBy = OrderByExpression;
        }


        #endregion

        #region OrderByDesc

        public Expression<Func<TEntity, object>>? OrderByDesc { get; private set; }
        protected void AddOrderByDesc(Expression<Func<TEntity, object>> OrderByDescExpression)
        {
            OrderByDesc = OrderByDescExpression;
        }


        #endregion

        #region Pagination [ Skip - Take ]



        public int Skip { get; private set; }

        public int Take { get; private set; }

        public bool IsPaginated { get; private set; }


        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPaginated = true;
            Take = pageSize;                    // product هتاخد كام 
            Skip = (pageIndex - 1) * pageSize;  // لكام صفحه Skip هتعمل 
        }


        #endregion


    }
}
