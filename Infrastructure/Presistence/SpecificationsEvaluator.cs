using Domain.Entites;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Presistence
{
    public static class SpecificationsEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity,TKey> 
                    (IQueryable<TEntity> BaseQuery , ISpecifications<TEntity,TKey> specifications) where TEntity : BaseEntity<TKey>
        {
            var query = BaseQuery;  // select * from Tasks

            if(specifications is not null)
            {

                // where 
                if (specifications.Criteria is not null) 
                    query = query.Where(specifications.Criteria);   // select * from Tasks where Tasks < 5

                // Includes
                if (specifications.Includes is not null && specifications.Includes.Any())
                {
                    foreach (var allIncludes in specifications.Includes)
                    {
                        query = query.Include(allIncludes); // select * from project Include Tasks Include Comment 
                    }
                }

                // OrderBy
                if (specifications.OrderBy is not null) 
                    query = query.OrderBy(specifications.OrderBy); // select * from project Include Tasks OrderBy name

                // OrderByDesc
                if (specifications.OrderByDesc is not null)
                   query = query.OrderByDescending(specifications.OrderByDesc);  // select * from project Include Tasks OrderByDesc name


                // Paginated
                if (specifications.IsPaginated is true)    // select * from project Skip 2 and take 5
                    query = query.Skip(specifications.Skip).Take(specifications.Take);



               

            }
           
            return query;
        }
    }
}
