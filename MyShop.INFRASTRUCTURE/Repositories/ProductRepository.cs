using Microsoft.EntityFrameworkCore;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        public async Task<List<Product>> HotSelledProductsAsync(int NumberOfProducts)
        {
            return await _context.Products
                .Include(p => p.productPhotos.Where(p => p.IsMain))
                .Include(p => p.productVariants.Take(1))
                .Include(p => p.Category)
                .OrderByDescending(p => p.Popularity)
                .Take(NumberOfProducts)
                .ToListAsync();

        }
        public  async Task<IEnumerable<Product>> test(
    Expression<Func<Product, bool>>? criteria,
    int? skip,
    int? take,
    Expression<Func<Product, object>>? orderBy = null,
    OrderByOptions orderByDirection = OrderByOptions.Ascending,
    params Expression<Func<Product, object>>[] includes)
        {
            IQueryable<Product> query = _context.Set<Product>();

            if (criteria is not null)
                query = query.Where(criteria);

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    var path = ParsePath(include);
                    if (!string.IsNullOrEmpty(path))
                        query = query.Include(path);
                    else
                        query = query.Include(include);
                }
            }

            if (orderBy is not null)
            {
                query = orderByDirection == OrderByOptions.Ascending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        private string ParsePath(Expression expression)
        {
            if (expression is LambdaExpression lambda)
                return ParsePath(lambda.Body);
            
            if (expression is MemberExpression member)
            {
                var parent = ParsePath(member.Expression);
                return string.IsNullOrEmpty(parent) ? member.Member.Name : $"{parent}.{member.Member.Name}";
            }

            if (expression is MethodCallExpression methodCall)
            {
                if (methodCall.Method.Name == "Select" || methodCall.Method.Name == "SelectMany")
                {
                    var parent = ParsePath(methodCall.Arguments[0]);
                    var child = ParsePath(methodCall.Arguments[1]);
                    return $"{parent}.{child}";
                }
            }
            
            return string.Empty;
        }
    }
}
