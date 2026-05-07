using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.Enums;
using MyShop.INFRASTRUCTURE.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.Domain.Shared;
using MyShop.Domain.Entities;
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

        public IQueryable<T> Query(bool asNoTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (asNoTracking)
                query = query.AsNoTracking();
            
            return query;
        }

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

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
