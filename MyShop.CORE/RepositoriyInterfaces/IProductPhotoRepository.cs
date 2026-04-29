using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.RepositoriyInterfaces
{
    public interface IProductPhotoRepository:IBaseRepository<ProductPhoto>
    {
        //Task<Result<ProductPhoto>> UploadProductPhotoAsync(ProductPhoto productPhoto);
    }
}
