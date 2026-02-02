using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;

namespace Wasalni.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> Set;
        public Repository(AppDbContext appDbContext)
        {
            this._db = appDbContext;
            this.Set = appDbContext.Set<T>();
        }
        public void Add(T entity)
        {
            Set.Add(entity);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;

            if (tracked)
                query = Set;
            else
                query = Set.AsNoTracking();
            query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }

            }
            return query.FirstOrDefault()!;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
                query = Set;
            else
                query = Set.AsNoTracking();
            
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }

            }
            if (filter != null)
                query = query.Where(filter);

            return query.ToList();
        }

        public void Remove(T entity)
        {
            Set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Set.RemoveRange(entities);
        }
    }
}
