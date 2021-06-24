using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.API.Core;
using Storage.API.Models;

namespace Storage.API.EF
{
    public class Repository<T> : IRepository<T> where T : PaginatedEntities
    {
        private readonly ApplicationDbContext _dbContext;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(T data)
        {
            await _dbContext.AddAsync(data);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Any(predicate);
        }

        public void Delete(T data)
        {
            _dbContext.Remove(data);
        }

        public async Task<T> FindAsync(object key)
        {
            return await _dbContext.Set<T>().FindAsync(key);
        }
        
        public async Task<T> GetAsync<TProperty>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProperty>>[] includePaths)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            query = includePaths.Aggregate(query, (current, path) => current.Include(path));
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<List<T>> ToListAsync<TProperty>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProperty>>[] includePaths)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            query = includePaths.Aggregate(query, (current, path) => current.Include(path));
            return await query.ToListAsync();
        }

        public async Task<List<T>> ToListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<Paginated<T>> ToPaginatedAsync(int limit, int count)
        {
            var query = _dbContext.Set<T>();
            var queryCount = query.Count();

            return new Paginated<T>()
            {
                Data = await query
                    .OrderByDescending(x => x.InsertDate)
                    .Skip((count - 1) * limit)
                    .Take(limit)
                    .ToListAsync(),
                Count = count,
                Limit = limit,
                Total = queryCount == 0 ? 0 : queryCount / limit + 1
            };
        }
        
        public async Task<Paginated<T>> ToPaginatedAsync<TProperty>(int limit, int count, params Expression<Func<T, TProperty>>[] includePath)
        {
            var query = _dbContext.Set<T>();
            var queryCount = query.Count();
            
            var pagedQuery = query
                .OrderByDescending(x => x.InsertDate)
                .Skip((count - 1) * limit)
                .Take(limit);
            pagedQuery = includePath.Aggregate(pagedQuery, (current, path) => current.Include(path));

            return new Paginated<T>()
            {
                Data = await pagedQuery
                    .ToListAsync(),
                Count = count,
                Limit = limit,
                Total = queryCount == 0 ? 0 : queryCount / limit + 1
            };
        }
        
        public async Task<Paginated<T>> ToPaginatedAsync(Expression<Func<T, bool>> predicate, int limit, int count)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            var queryCount = query.Count();
            return new Paginated<T>()
            {
                Data = await query
                    .OrderByDescending(x => x.InsertDate)
                    .Skip((count - 1) * limit)
                    .Take(limit)
                    .ToListAsync(),
                Count = count,
                Limit = limit,
                Total = queryCount == 0 ? 0 : queryCount / limit + 1
            };
        }
        
        public async Task<Paginated<T>> ToPaginatedAsync<TProperty>(Expression<Func<T, bool>> predicate, int limit, int count, params Expression<Func<T, TProperty>>[] includePath)
        {
            var query = _dbContext.Set<T>().Where(predicate);
            var queryCount = query.Count();
            
            var pagedQuery = query
                .OrderByDescending(x => x.InsertDate)
                .Skip((count - 1) * limit)
                .Take(limit);
            pagedQuery = includePath.Aggregate(pagedQuery, (current, path) => current.Include(path));
            
            return new Paginated<T>()
            {
                Data = await pagedQuery
                    .ToListAsync(),
                Count = count,
                Limit = limit,
                Total = queryCount == 0 ? 0 : queryCount / limit + 1
            };
        }

        public void Update(T data)
        {
            _dbContext.Update(data);
        }
    }
}
