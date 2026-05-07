using MyShop.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using MyShop.Domain.Entities;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MyShop.Application.DTOs.Category;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<bool> IsParentAsync(Guid Category, Guid ParentCategory)
        {
            Guid? category = Category;
            while(category is not null)
            {
                if(category == ParentCategory)
                    return true;
                category = await _context.Categories.Where(c => c.Id == category).Select(c => c.SuperCategoryId).FirstOrDefaultAsync();
            }
            return false;
        }
        public async Task<List<Guid>> GetSelfAndDescendantIdsAsync(Guid categoryId)
        {
            var result = new List<Guid> { categoryId };
            var queue = new Queue<Guid>();
            queue.Enqueue(categoryId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                var children = await _context.Categories
                    .Where(c => c.SuperCategoryId == current)
                    .Select(c => c.Id)
                    .ToListAsync();

                foreach (var child in children)
                {
                    result.Add(child);
                    queue.Enqueue(child);
                }
            }

            return result;
        }
    }
}
