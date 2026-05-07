using MyShop.Domain.Entities.ProductEntities;
using MyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IProductRepository:IBaseRepository<Product>
    {

    }
}
