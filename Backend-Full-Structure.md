# MyShop Backend: Complete Project Structure (All Layers)

This document contains a comprehensive list of EVERY file in the backend project, organized by project and sub-folder.

---

## 🌐 MyShop.API (Web API Layer)
```text
MyShop.API
 ├── [Controllers]
 │    ├── AuthController.cs
 │    ├── CartItemsController.cs
 │    ├── CartsController.cs
 │    ├── CategoriesController.cs
 │    ├── CouponsController.cs
 │    ├── OrdersController.cs
 │    ├── ProductsController.cs
 │    ├── ProfileController.cs
 │    ├── ReviewsController.cs
 │    └── WishListController.cs
 ├── [Hubs]
 │    └── OrderHub.cs
 ├── [Middlewares]
 │    └── ExceptionMiddleware.cs
 ├── [Properties]
 │    └── launchSettings.json
 ├── [Services]
 │    └── SignalRNotificationService.cs
 ├── [wwwroot]
 │    ├── [Photos]
 │    └── [UserPhotos]
 ├── Program.cs
 ├── appsettings.json
 ├── appsettings.Development.json
 └── MyShop.API.csproj
```

---

## 🧠 MyShop.CORE (Business Logic Layer)
```text
MyShop.CORE
 ├── [AutoMapping]
 │    ├── CartProfile.cs
 │    ├── CategoryProfile.cs
 │    ├── CouponProfile.cs
 │    ├── OrderProfile.cs
 │    ├── ProductProfile.cs
 │    ├── ReviewProfile.cs
 │    ├── UserProfile.cs
 │    └── WishProfile.cs
 ├── [Dtos]
 │    ├── [Auth]
 │    │    ├── AuthResponseDto.cs
 │    │    ├── ConfirmEmailDto.cs
 │    │    ├── ForgotPasswordDto.cs
 │    │    ├── GoogleLoginDto.cs
 │    │    ├── LoginDto.cs
 │    │    ├── RefreshTokenDto.cs
 │    │    ├── RegisterDto.cs
 │    │    ├── ResendEmailConfirmationDto.cs
 │    │    └── ResetPasswordDto.cs
 │    ├── [Cart]
 │    │    └── CartDto.cs
 │    ├── [CartItem]
 │    │    ├── CartItemCreateDto.cs
 │    │    ├── CartItemDto.cs
 │    │    └── CartItemUpdateDto.cs
 │    ├── [Category]
 │    │    ├── AddCategoryDto.cs
 │    │    └── GetCategoryDto.cs
 │    ├── [Coupon]
 │    │    ├── AssignCouponDto.cs
 │    │    ├── BulkAssignCouponDto.cs
 │    │    ├── CouponDto.cs
 │    │    ├── CouponResponseDto.cs
 │    │    ├── CreateCouponDto.cs
 │    │    ├── UpdateCouponDto.cs
 │    │    └── UserCouponDto.cs
 │    ├── [Identity]
 │    │    └── CreateIdentityUserDto.cs
 │    ├── [Order]
 │    │    ├── AddOrderDto.cs
 │    │    ├── OrderDto.cs
 │    │    └── UpdateOrderStatusDto.cs
 │    ├── [Product]
 │    │    ├── AddProductDto.cs
 │    │    ├── AddProductVariantDto.cs
 │    │    ├── AddVariantAttributeDto.cs
 │    │    ├── ProductDto.cs
 │    │    ├── ProductPhotoDto.cs
 │    │    ├── ProductVariantDto.cs
 │    │    ├── UpdateProductDto.cs
 │    │    └── VariantAttributeDto.cs
 │    ├── [Profile]
 │    │    ├── ChangePasswordDto.cs
 │    │    ├── ProfileDto.cs
 │    │    └── UpdateProfileDto.cs
 │    ├── [Review]
 │    │    ├── AddReviewDto.cs
 │    │    └── ReviewResponseDto.cs
 │    └── [Wish]
 │         └── WishDto.cs
 ├── [Entities]
 │    ├── [OrderEntities]
 │    │    ├── Order.cs
 │    │    └── OrderItem.cs
 │    ├── Attribute.cs
 │    ├── CartItem.cs
 │    ├── Category.cs
 │    ├── Coupon.cs
 │    ├── Customer.cs
 │    ├── Product.cs
 │    ├── ProductCoupon.cs
 │    ├── ProductPhoto.cs
 │    ├── ProductVariant.cs
 │    ├── Review.cs
 │    ├── Seller.cs
 │    ├── UserCoupon.cs
 │    ├── UserPhoto.cs
 │    ├── VariantAttribute.cs
 │    └── WishList.cs
 ├── [Enums]
 │    ├── CitiesOptions.cs
 │    ├── DeliveryStatusOptions.cs
 │    ├── DiscountType.cs
 │    ├── OrderByOptions.cs
 │    ├── RequestExecution.cs
 │    └── RoleOptions.cs
 ├── [FluentValidation]
 │    ├── [Auth]
 │    │    ├── LoginDtoValidator.cs
 │    │    ├── RegisterDtoValidator.cs
 │    │    └── TokenModelDtoValidator.cs
 │    ├── [Cart]
 │    │    └── CartItemDtoValidator.cs
 │    ├── [Category]
 │    │    └── AddCategoryDtoValidator.cs
 │    ├── [Order]
 │    │    └── AddOrderDtoValidator.cs
 │    ├── [Product]
 │    │    ├── AddProductDtoValidator.cs
 │    │    └── UpdateProductDtoValidator.cs
 │    ├── [Profile]
 │    │    ├── ChangePasswordDtoValidator.cs
 │    │    └── UpdateProfileDtoValidator.cs
 │    └── [Review]
 │         └── AddReviewDtoValidator.cs
 ├── [Helpers]
 │    └── [ResultPattern]
 │         ├── BaseResponse.cs
 │         ├── Error.cs
 │         ├── PageResult.cs
 │         └── Result.cs
 ├── [Identity]
 │    ├── ApplicationRole.cs
 │    ├── ApplicationUser.cs
 │    └── RefreshToken.cs
 ├── [Implmentations]
 │    ├── [Auth]
 │    │    └── AuthService.cs
 │    ├── CartItemsService.cs
 │    ├── CartService.cs
 │    ├── CategoryService.cs
 │    ├── CouponService.cs
 │    ├── FileService.cs
 │    ├── OrderService.cs
 │    ├── ProductService.cs
 │    ├── ProfileService.cs
 │    ├── ReviewService.cs
 │    └── WishService.cs
 ├── [Interfaces]
 │    ├── [Auth]
 │    │    ├── IAuthService.cs
 │    │    ├── IEmailService.cs
 │    │    └── ITokenService.cs
 │    ├── ICacheService.cs
 │    ├── ICartItemsService.cs
 │    ├── ICartService.cs
 │    ├── ICategoryService.cs
 │    ├── ICouponService.cs
 │    ├── IFileService.cs
 │    ├── IIdentityService.cs
 │    ├── INotificationService.cs
 │    ├── IOrderService.cs
 │    ├── IProductService.cs
 │    ├── IProfileService.cs
 │    ├── IReviewService.cs
 │    ├── IUnitOfWork.cs
 │    └── IWishService.cs
 ├── [RepositoriyInterfaces]
 │    ├── IAttributeRepository.cs
 │    ├── IBaseRepository.cs
 │    ├── ICartItemRepository.cs
 │    ├── ICategoryRepository.cs
 │    ├── ICouponRepository.cs
 │    ├── ICustomerRepository.cs
 │    ├── IOrderItemRepository.cs
 │    ├── IOrderRepository.cs
 │    ├── IProductCouponRepository.cs
 │    ├── IProductPhotoRepository.cs
 │    ├── IProductRepository.cs
 │    ├── IProductVariantRepository.cs
 │    ├── IRefreshTokenRepository.cs
 │    ├── IReviewRepository.cs
 │    ├── ISellerRepository.cs
 │    ├── IUserCouponRepository.cs
 │    ├── IUserPhotoRepository.cs
 │    ├── IUserRepository.cs
 │    ├── IVariantAttributeRepository.cs
 │    └── IWishRepository.cs
 ├── [Settings]
 │    ├── ClientSettings.cs
 │    ├── EmailSettings.cs
 │    ├── GoogleSettings.cs
 │    └── JwtSettings.cs
 ├── [Shared]
 │    └── SearchFilterOptions.cs
 ├── CoreRegisteration.cs
 └── MyShop.CORE.csproj
```

---

## 🛠️ MyShop.INFRASTRUCTURE (Infrastructure Layer)
```text
MyShop.INFRASTRUCTURE
 ├── [Context]
 │    └── AppDbContext.cs
 ├── [Configs]
 │    ├── AppUserConfiguration.cs
 │    ├── CartItemConfiguration.cs
 │    ├── CouponConfiguration.cs
 │    ├── CustomerConfiguration.cs
 │    ├── OrderConfiguration.cs
 │    ├── OrderItemConfiguration.cs
 │    ├── ProductConfiguration.cs
 │    ├── ProductCouponConfig.cs
 │    └── WishConfiguration.cs
 ├── [Repositories]
 │    ├── AttributeRepository.cs
 │    ├── BaseRepository.cs
 │    ├── CartItemRepository.cs
 │    ├── CategoryRepository.cs
 │    ├── CouponRepository.cs
 │    ├── CustomerRepository.cs
 │    ├── OrderItemRepository.cs
 │    ├── OrderRepository.cs
 │    ├── ProductCouponRepository.cs
 │    ├── ProductPhotoRepository.cs
 │    ├── ProductRepository.cs
 │    ├── ProductVariantRepository.cs
 │    ├── RefreshTokenRepository.cs
 │    ├── ReviewRepository.cs
 │    ├── SellerRepository.cs
 │    ├── UnitOfWork.cs
 │    ├── UserCouponRepository.cs
 │    ├── UserPhotoRepository.cs
 │    ├── UserRepository.cs
 │    ├── VariantAttributeRepository.cs
 │    └── WishRepository.cs
 ├── [Services]
 │    ├── CacheService.cs
 │    ├── DbSeeder.cs
 │    ├── EmailService.cs
 │    ├── IdentityService.cs
 │    └── TokenService.cs
 ├── InfrastructureRegisteration.cs
 └── MyShop.INFRASTRUCTURE.csproj
```

---

## 📄 Solution Files
- **MyShop.sln**: Main solution file.
- **Backend-Architecture.md**: High-level flow.
- **Backend-Brief.md**: File descriptions.
- **Backend-Structure.md**: Visual summary.
