using System.Linq.Expressions;

namespace Wasalni.Infrastructure.Specifications
{
    public abstract class Specification<T> where T : class
    {
        public bool IsSplitQuery {get; set; }
        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public List<Expression<Func<T, object>>>? includesExpressions { get; private set; }
        public Expression<Func<T,object>>? OrderByExpression { get; private set; }
        public Expression<Func<T,object>>? OrderByDescendingExpression { get; private set; }
        protected Specification(Expression<Func<T, bool>>? criteriaExp)
        {
            Criteria = criteriaExp;
            includesExpressions = new List<Expression<Func<T, object>>>();
        }
        public void AddInclude(Expression<Func<T, object>> include) => includesExpressions.Add(include);
        public void OrderBy(Expression<Func<T, object>> orderBy) => OrderByExpression = orderBy;
        public void OrderByDescending(Expression<Func<T, object>> orderBy) => OrderByDescendingExpression = orderBy;
    }
}
