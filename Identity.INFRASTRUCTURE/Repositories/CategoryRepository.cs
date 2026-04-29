using Microsoft.EntityFrameworkCore;
using MyShop.CORE.Dtos.Category;
using MyShop.CORE.Entities;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<List<GetCategoryDto>> GetCategoriesWithProductsCount()
        {
            return await _context.Categories
                .Where(c=>c.SuperCategory!=null)
                .Select(c => new GetCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductsCount = c.Products.Count()
                })
                .ToListAsync();
        }
    }
}
