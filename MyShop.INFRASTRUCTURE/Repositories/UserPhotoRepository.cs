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
    public class UserPhotoRepository : BaseRepository<UserPhoto>, IUserPhotoRepository
    {
        public UserPhotoRepository(AppDbContext appDbContext) : base(appDbContext)
        {
        }
    }
}
