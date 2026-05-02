using MyShop.CORE.Dtos.Wish;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface IWishService
    {
        Task<Result<WishDto>> AddWish(Guid userId, WishDto wish);
        Task<Result<List<WishDto>>> GetWishesByUserId(Guid userId);
        Task<Result<bool>> RemoveWish(Guid userId, WishDto wish);
    }
}
