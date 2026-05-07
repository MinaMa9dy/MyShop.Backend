using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Domain.Entities.ProductEntities;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class ProductVariantRepository : BaseRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
