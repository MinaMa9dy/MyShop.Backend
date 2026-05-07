using MyShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<bool> IsParentAsync(Guid Category, Guid ParentCategory);
        Task<List<Guid>> GetSelfAndDescendantIdsAsync(Guid categoryId);
    }
    
}
