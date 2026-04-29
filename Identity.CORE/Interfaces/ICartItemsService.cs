using MyShop.CORE.Dtos.CartItem;
using MyShop.CORE.Helpers.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.CORE.Interfaces
{
    public interface ICartItemsService
    {
        // User methods
        Task<Result<CartItemDto>> AddToCartAsync(Guid userId, CartItemCreateDto dto);
        Task<Result<CartItemDto>> UpdateQuantityAsync(Guid userId, CartItemUpdateDto dto);
        Task<Result<bool>> RemoveFromCartAsync(Guid userId, Guid productVariantId);
    }
}
