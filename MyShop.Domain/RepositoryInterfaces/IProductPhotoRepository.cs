using MyShop.Domain.Entities.ProductEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IProductPhotoRepository:IBaseRepository<ProductPhoto>
    {
        //Task<Result<ProductPhoto>> UploadProductPhotoAsync(ProductPhoto productPhoto);
    }
}
