using Microsoft.EntityFrameworkCore;
using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .Include(p => p.Supplier).ThenInclude(s => s.User)
                .OrderByDescending(p => p.Popularity)
                .Take(NumberOfProducts)
                .ToListAsync();

        }
        
    }
}
