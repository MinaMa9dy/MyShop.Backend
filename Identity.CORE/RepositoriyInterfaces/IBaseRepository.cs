using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using MyShop.CORE.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.RepositoriyInterfaces
{
    public interface IBaseRepository<T> where T : class
    {
        T? GetById(int id);
        Task<T?> GetByIdAsync(int id);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T? Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T?> FindAsync(Expression<Func<T, bool>>? criteria, string[] includes = null);
        IEnumerable<T> FindAll(Expression<Func<T, bool>>? criteria, int? skip, int? take,
            Expression<Func<T, object>>? orderBy = null, OrderByOptions orderByDirection = OrderByOptions.Ascending, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null, int? skip = null, int? take = null,
            Expression<Func<T, object>>? orderBy = null, OrderByOptions orderByDirection = OrderByOptions.Ascending, string[] includes = null);


        void Add(T entity);
        Task AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    }
}
