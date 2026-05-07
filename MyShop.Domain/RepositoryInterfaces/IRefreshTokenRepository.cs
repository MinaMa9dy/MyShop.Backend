using MyShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IRefreshTokenRepository:IBaseRepository<RefreshToken>
    {
    }
}
