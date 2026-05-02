# MyShop Backend Architecture & File Structure

This document provides a comprehensive visual map of the backend project, following the Clean Architecture pattern with three main layers: **API**, **CORE**, and **INFRASTRUCTURE**.

## 🏗️ Visual Architecture Map

```mermaid
graph TD
    subgraph "Presentation Layer (API)"
        API[MyShop.API] --> Controllers[Controllers]
        API --> Middlewares[Middlewares]
        API --> Config[appsettings.json / Program.cs]
    end

    subgraph "Business Logic Layer (CORE)"
        CORE[MyShop.CORE] --> Entities[Entities / Models]
        CORE --> DTOs[DTOs]
        CORE --> Interfaces[Interfaces / Repositories]
        CORE --> Services[Services / Logic]
        CORE --> Mapping[AutoMapping]
    end

    subgraph "Infrastructure Layer (INFRA)"
        INFRA[MyShop.INFRASTRUCTURE] --> Context[AppDbContext]
        INFRA --> Repos[Repositories Impl]
        INFRA --> Migrations[EF Core Migrations]
        INFRA --> Configs[EF Entity Configs]
        INFRA --> ExtServices[External Service Impl]
    end

    Controllers --> Services
    Services --> Interfaces
    Repos -- Implements --> Interfaces
    Context --> Entities
```

---

## 📂 Detailed File Structure

### 🌐 MyShop.API
*The entry point of the application, handling HTTP requests and routing.*
- 📁 **Controllers/**: REST Endpoints (Auth, Products, Cart, Orders, etc.)
- 📁 **Middlewares/**: Exception handling, Logging, Auth filters.
- 📄 **Program.cs**: Dependency Injection and Middleware pipeline configuration.
- 📄 **appsettings.json**: Configuration for Connection Strings, JWT, and SMTP.

### 🧠 MyShop.CORE
*The heart of the application, containing business rules and domain logic.*
- 📁 **Entities/**: Database models (Product, User, Order, Category).
- 📁 **Interfaces/**: Abstract definitions for Repositories and Services.
- 📁 **DTOs/**: Data Transfer Objects for clean request/response handling.
- 📁 **Services/**: Implementation of core business workflows.
- 📁 **AutoMapping/**: Profile configurations for mapping Entities to DTOs.
- 📁 **Consts/**: Global constants and enums (Roles, Categories).

### 🛠️ MyShop.INFRASTRUCTURE
*Handles data persistence and communication with external systems.*
- 📁 **Context/**: `AppDbContext` for Entity Framework Core.
- 📁 **Repositories/**: Concrete implementations of the CORE interfaces (UnitOfWork, CRUD).
- 📁 **Configs/**: Fluent API configurations for database schema (Relationships, Indexes).
- 📁 **Migrations/**: Version history of the database schema.
- 📁 **Services/**: Implementations for Token generation, Emailing, and File Storage.

---

## 🛠️ Technology Stack
- **Framework**: .NET 9.0
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Auth**: JWT (JSON Web Tokens) & ASP.NET Identity
- **Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI
