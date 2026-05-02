using MyShop.CORE.Dtos.Product;
using MyShop.CORE.Entities;
using MyShop.CORE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.RepositoriyInterfaces
{
    public interface IProductRepository:IBaseRepository<Product>
    {
        Task<List<Product>> HotSelledProductsAsync(int NumberOfProducts);
        Task<IEnumerable<Product>> test(
    Expression<Func<Product, bool>>? criteria = null,
    int? take = null,
    int? skip = null,
    Expression<Func<Product, object>>? orderBy = null,
    OrderByOptions orderByDirection = OrderByOptions.Ascending,
    params Expression<Func<Product, object>>[] includes);
    }
}
