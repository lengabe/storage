using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Storage.API.Models;

namespace Storage.API.Core
{
    public interface IRepository<T>
    {
        Task AddAsync(T data);
        void Update(T data);
        void Delete(T data);
        Task<T> FindAsync(object key);
        Task<T> GetAsync<TProperty>(Expression<Func<T, bool>> predicate, params Expression<Func<T, TProperty>>[] includePaths);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task<Paginated<T>> ToPaginatedAsync(int limit, int count);
        Task<Paginated<T>> ToPaginatedAsync<TProperty>(int limit, int count, params Expression<Func<T, TProperty>>[] includePath);
        Task<Paginated<T>> ToPaginatedAsync(Expression<Func<T, bool>> predicate, int limit, int count);
        Task<Paginated<T>> ToPaginatedAsync<TProperty>(Expression<Func<T, bool>> predicate, int limit, int count, params Expression<Func<T, TProperty>>[] includePath);
        Task SaveChangesAsync();
    }
}
