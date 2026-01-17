using Microsoft.EntityFrameworkCore;

namespace Wasalni.Infrastructure.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<T> applySpecification<T>(IQueryable<T> set, Specification<T> specification) where T : class
        {
            IQueryable<T> entities = set;
            if (specification.Criteria is not null)
            {
                entities = entities.Where(specification.Criteria);
            }
            if (specification.includesExpressions != null && specification.includesExpressions.Any())
            {
                foreach (var include in specification.includesExpressions)
                {
                    entities = entities.Include(include);
                }
            }
            if (specification.OrderByExpression is not null)
            {
                entities = entities.OrderBy(specification.OrderByExpression);
            }
            else if (specification.OrderByDescendingExpression is not null)
            {
                entities = entities.OrderByDescending(specification.OrderByDescendingExpression);
            }
            if (specification.IsSplitQuery)
                entities = entities.AsSplitQuery();
            return entities;
        }
    }
}
