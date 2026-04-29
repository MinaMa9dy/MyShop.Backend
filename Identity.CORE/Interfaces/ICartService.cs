using Microsoft.AspNetCore.Http.HttpResults;
using MyShop.CORE.Dtos.Cart;
using MyShop.CORE.Dtos.CartItem;
using MyShop.CORE.Entities;
using MyShop.CORE.Helpers;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartDto>> GetByUserIdAsync(Guid userId);
        Task<Result<bool>> ClearCartAsync(Guid userId);

        //// Admin endpoints
        Task<Result<PageResult<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10);





        

    }
}
