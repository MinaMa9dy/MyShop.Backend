using MyShop.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Domain.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        
        //ICitiesRepository Cities { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IUserRepository Users { get; }
        ICartItemRepository CartItems { get; }
        IReviewRepository Reviews { get; }
        IUserPhotoRepository UserPhotos { get; }
        IOrderRepository Orders { get; }
        IProductPhotoRepository ProductPhotos { get; }
        IOrderItemRepository OrderItems { get; }
        IWishRepository Wishes { get; }
        ISellerRepository Sellers { get; }
        ICustomerRepository Customers { get; }
        ICouponRepository Coupons { get; }
        IProductCouponRepository ProductCoupons { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IUserCouponRepository UserCoupons { get; }
        IProductVariantRepository ProductVariants { get; }
        IVariantAttributeRepository VariantAttributes { get; }
        IAttributeRepository Attributes { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}
