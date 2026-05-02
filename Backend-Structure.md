# MyShop Backend: Project Structure

This document provides a clean visual representation of the file structure across the entire backend solution.

---

## 🌐 MyShop.API
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
 ├── Program.cs
 ├── appsettings.json
 └── MyShop.API.csproj
```

---

## 🧠 MyShop.CORE
```text
MyShop.CORE
 ├── [AutoMapping]
 │    ├── ProductProfile.cs
 │    ├── UserProfile.cs
 │    └── ... (Order, Category, Coupon Profiles)
 ├── [Dtos]
 │    ├── [Auth]
 │    │    ├── LoginDto.cs
 │    │    └── RegisterDto.cs
 │    ├── [Product]
 │    │    ├── ProductDto.cs
 │    │    └── AddProductDto.cs
 │    └── ... (Cart, Order, Category Dtos)
 ├── [Entities]
 │    ├── Product.cs
 │    ├── ApplicationUser.cs
 │    ├── Order.cs
 │    ├── Category.cs
 │    └── ProductVariant.cs
 ├── [Enums]
 │    ├── RoleOptions.cs
 │    └── OrderByOptions.cs
 ├── [Interfaces]
 │    ├── IProductService.cs
 │    ├── IAuthService.cs
 │    └── IUnitOfWork.cs
 ├── [RepositoriyInterfaces]
 │    ├── IProductRepository.cs
 │    └── IUserRepository.cs
 └── MyShop.CORE.csproj
```

---

## 🛠️ MyShop.INFRASTRUCTURE
```text
MyShop.INFRASTRUCTURE
 ├── [Context]
 │    └── AppDbContext.cs
 ├── [Configs]
 │    ├── ProductConfiguration.cs
 │    └── OrderConfiguration.cs
 ├── [Repositories]
 │    ├── ProductRepository.cs
 │    ├── UserRepository.cs
 │    └── BaseRepository.cs
 ├── [Services]
 │    ├── TokenService.cs
 │    ├── EmailService.cs
 │    └── DbSeeder.cs
 └── MyShop.INFRASTRUCTURE.csproj
```

---

## 📄 Root Files
- **MyShop.sln**: The visual studio solution file.
- **Backend-Architecture.md**: High-level architecture guide.
- **Backend-Brief.md**: Description of what each file does.
