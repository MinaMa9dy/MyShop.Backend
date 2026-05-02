# MyShop Backend: Detailed File Brief

This document provides a line-by-line brief for the core files in the backend project, organized by layer.

---

## 🌐 1. MyShop.API (Presentation Layer)
*The entry point, responsible for HTTP communication and middleware.*

### 📂 Controllers
- `AuthController.cs`: Manages user authentication, registration, and token refresh.
- `CartItemsController.cs`: Handles operations on individual items within a shopping cart.
- `CartsController.cs`: Manages the overall shopping cart lifecycle.
- `CategoriesController.cs`: CRUD operations for product categories.
- `CouponsController.cs`: Manages discount coupons and their application.
- `OrdersController.cs`: Processes order placement, history, and status updates.
- `ProductsController.cs`: Main endpoint for product catalog, search, and details.
- `ProfileController.cs`: Manages user profile details and password changes.
- `ReviewsController.cs`: Handles product reviews and ratings.
- `WishListController.cs`: Manages user wishlists.

### 📁 Root Files
- `Program.cs`: Bootstraps the application, configures DI, and defines the middleware pipeline.
- `appsettings.json`: Stores configuration strings, API keys, and environment-specific settings.

---

## 🧠 2. MyShop.CORE (Domain & Logic Layer)
*Contains entities, business logic, and interface definitions.*

### 📂 Entities
- `ApplicationUser.cs`: Extended Identity user model.
- `Product.cs`: Core product model with price and stock details.
- `Order.cs`: Customer order details and status.
- `Category.cs`: Product category grouping.
- `CartItem.cs`: Link between users and their selected products.
- `ProductVariant.cs`: Handles specific product options (size, color).
- `Coupon.cs`: Definition of discount codes and rules.

### 📂 DTOs (Data Transfer Objects)
- `Auth/LoginDto.cs`: Schema for login requests.
- `Product/ProductResponseDto.cs`: Cleaned product data for frontend display.
- `Order/AddOrderDto.cs`: Schema for submitting a new order.
- `Cart/CartItemDto.cs`: Represents an item in the cart API response.

### 📂 Interfaces & Implementations
- `IProductService.cs` / `ProductService.cs`: Logic for fetching, searching, and managing products.
- `IAuthService.cs` / `AuthService.cs`: Business logic for login, registration, and role management.
- `IOrderService.cs` / `OrderService.cs`: Workflow for processing orders and managing stock.
- `ICartService.cs` / `CartService.cs`: Logic for managing user carts.
- `IUnitOfWork.cs`: Orchestrates multiple repository operations in a single transaction.

### 📂 Helpers & Enums
- `Result.cs`: Standardized API response wrapper (isSuccess, Data, Error).
- `PageResult.cs`: Wrapper for paginated data responses.
- `RoleOptions.cs`: Enumeration of user roles (Admin, Seller, Customer).
- `CitiesOptions.cs`: List of supported cities for delivery.

---

## 🛠️ 3. MyShop.INFRASTRUCTURE (Data Layer)
*Handles physical data storage and external integrations.*

### 📂 Context & Configs
- `AppDbContext.cs`: The Entity Framework database context.
- `ProductConfiguration.cs`: Database schema rules (indexes, constraints) for Products.
- `OrderConfiguration.cs`: Relationship mapping for Orders and Items.

### 📂 Repositories (Implementation)
- `BaseRepository.cs`: Generic CRUD implementation used by all repositories.
- `ProductRepository.cs`: Optimized queries for products including search and hot-sales.
- `UserRepository.cs`: Direct database access for identity and profile data.
- `UnitOfWork.cs`: Implementation of the transactional pattern.

### 📂 Services (External)
- `TokenService.cs`: Generates and validates JWT and Refresh tokens.
- `EmailService.cs`: Logic for sending verification and password reset emails.
- `DbSeeder.cs`: Populates the database with initial categories and roles.
