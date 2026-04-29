using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.RepositoriyInterfaces
{
    public interface IProductRepository:IBaseRepository<Product>
    {
        Task<List<Product>> HotSelledProductsAsync(int NumberOfProducts);
    }
}
