using MyShop.Application.DTOs.CartItem;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface ICartItemsService
    {
        // User methods
        Task<Result<CartItemDto>> AddToCartAsync(Guid userId, CartItemCreateDto dto);
        Task<Result<CartItemDto>> UpdateQuantityAsync(Guid userId, UpdateCartItemDto dto);
        Task<Result<bool>> RemoveFromCartAsync(Guid userId, Guid productVariantId);
    }
}
