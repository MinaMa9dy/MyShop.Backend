using MyShop.Domain.Identity;
using MyShop.Domain.RepositoryInterfaces;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IUserRepository : IBaseRepository<ApplicationUser>
    {
    }
}
