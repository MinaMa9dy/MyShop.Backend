using MyShop.Domain.Entities.OrderEntities;
using MyShop.Domain.Entities;
using MyShop.Application.Interfaces;
using MyShop.Domain.RepositoryInterfaces;
using MyShop.INFRASTRUCTURE.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.INFRASTRUCTURE.Repositories
{
    public class UnitOfWork : IUnitOfWork 
    {
        private readonly AppDbContext _context;

        //public ICitiesRepository Cities { get; }
        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }
        public ICartItemRepository CartItems { get; }
        public IReviewRepository Reviews { get; }
        public IUserPhotoRepository UserPhotos { get; }
        public IOrderRepository Orders { get; }
        public IProductPhotoRepository ProductPhotos { get; }
        public IOrderItemRepository OrderItems { get; }
        public IWishRepository Wishes { get; }
        public ICustomerRepository Customers { get; }
        public ISellerRepository Sellers { get; }
        public IProductCouponRepository ProductCoupons { get; }
        public ICouponRepository Coupons { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IAttributeRepository Attributes { get; }
        public IVariantAttributeRepository VariantAttributes { get; }
        public IProductVariantRepository ProductVariants { get; }
        public IUserCouponRepository UserCoupons { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Users = new UserRepository(_context);
            CartItems = new CartItemRepository(_context);
            Reviews = new ReviewRepository(_context);
            UserPhotos = new UserPhotoRepository(_context);
            Orders = new OrderRepository(_context);
            ProductPhotos = new ProductPhotoRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Wishes = new WishRepository(_context);
            Customers = new CustomerRepository(_context);
            Sellers = new SellerRepository(_context);
            ProductCoupons = new ProductCouponRepository(_context);
            Coupons = new CouponRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            Attributes = new AttributeRepository(_context);
            VariantAttributes = new VariantAttributeRepository(_context);
            ProductVariants = new ProductVariantRepository(_context);
            UserCoupons = new UserCouponRepository(_context);



        }

        public int Complete()
        {
            return _context.SaveChanges();
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
