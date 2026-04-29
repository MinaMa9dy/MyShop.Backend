using MyShop.CORE.Entities;
using MyShop.CORE.RepositoriyInterfaces;
using MyShop.INFRASTRUCTURE.Context;

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
