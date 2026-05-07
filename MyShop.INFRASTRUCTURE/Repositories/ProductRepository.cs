using MyShop.Domain.Entities.OrderEntities;
using Microsoft.EntityFrameworkCore;
using MyShop.Domain.Enums;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyShop.Domain.Entities.ProductEntities;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }

        
    }
}
