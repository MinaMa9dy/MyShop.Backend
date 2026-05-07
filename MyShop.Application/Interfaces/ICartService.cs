using Microsoft.AspNetCore.Http.HttpResults;
using MyShop.Application.DTOs.Cart;
using MyShop.Application.DTOs.CartItem;
using MyShop.Domain.Entities;
using MyShop.Application.Common;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface ICartService
    {
        Task<Result<CartDto>> GetByUserIdAsync(Guid userId);
        Task<Result<bool>> ClearCartAsync(Guid userId);

        //// Admin endpoints
        Task<Result<IEnumerable<CartDto>>> GetAllAsync(int page = 1, int pageSize = 10);
    }
}
