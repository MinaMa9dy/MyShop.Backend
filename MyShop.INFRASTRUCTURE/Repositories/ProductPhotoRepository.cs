using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using MyShop.Domain.Entities.ProductEntities;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class ProductPhotoRepository:BaseRepository<ProductPhoto>, IProductPhotoRepository
    {
        public ProductPhotoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
        public override void Delete(ProductPhoto entity)
        {
            base.Delete(entity);
        }
        


    }
}
