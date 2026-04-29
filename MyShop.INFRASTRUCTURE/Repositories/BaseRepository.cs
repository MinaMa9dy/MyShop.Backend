using MyShop.CORE.Enums;
using MyShop.INFRASTRUCTURE.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.CORE.Shared;
using MyShop.CORE.Entities;
using Microsoft.Identity.Client;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        public BaseRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
            
        }
        //Throw

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.AddRangeAsync(entities);
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> criteria)
        {
            return await _context.Set<T>().CountAsync(criteria);
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public T? Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.FirstOrDefault(criteria);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>>? criteria, int? skip, int? take,
            Expression<Func<T, object>>? orderBy = null, OrderByOptions orderByDirection = OrderByOptions.Ascending, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderByOptions.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            List<T> result = new List<T>();
            result = query.ToList();
            return result;
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? criteria, int? skip, int? take,
            Expression<Func<T, object>>? orderBy = null, OrderByOptions orderByDirection = OrderByOptions.Ascending, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (criteria is not null)
                query = query.Where(criteria);

            if (orderBy != null)
            {
                if (orderByDirection == OrderByOptions.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);
            List<T> result = new List<T>();
            result = await query.ToListAsync();
            return result;

        }

        public Task<T?> FindAsync(Expression<Func<T, bool>>? criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);
            return query.FirstOrDefaultAsync(criteria);
        }

        public IEnumerable<T> GetAll()
        {
            IEnumerable<T> values = new List<T>();
            values = _context.Set<T>().ToList();
            return values;

        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            IEnumerable<T> values = new List<T>();
            values = await _context.Set<T>().ToListAsync();
            return values;
        }

        public T? GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
