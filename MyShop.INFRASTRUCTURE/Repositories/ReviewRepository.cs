using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.Entities;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
