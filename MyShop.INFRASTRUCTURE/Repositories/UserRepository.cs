using MyShop.Domain.Entities;
using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.Identity;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
