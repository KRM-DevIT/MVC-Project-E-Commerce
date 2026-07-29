# ShopHub - ASP.NET Core MVC E-Commerce

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4)
![Architecture](https://img.shields.io/badge/Architecture-Layered-0A66C2)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-8.0-6C3483)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green)

**Live Demo:** [shopthehub.runasp.net](http://shopthehub.runasp.net/)

ShopHub is a full-stack e-commerce web application built with a **Layered Architecture** and **ASP.NET Core MVC**. It covers the complete shopping flow, from browsing products and managing a cart to checkout and order tracking, together with an administration area for managing the store.

I created this project to apply architectural separation and common design patterns in a realistic application while gaining hands-on experience with authentication, role-based authorization, session state, AJAX requests, and relational data.

## Architecture and design patterns

> **The project follows a Layered Architecture. The Presentation layer is built with ASP.NET Core MVC, the business logic is organized into a dedicated Service layer, and data persistence is handled through Entity Framework Core using the Repository and Unit of Work patterns.**

This distinction is important: MVC describes how web requests and UI concerns are organized, but it does not describe the structure of the complete application. ShopHub separates presentation, business workflows, and persistence into dedicated layers.

| Concept | Classification | Responsibility in ShopHub |
|---|---|---|
| **Layered Architecture** | Architectural style | Defines the overall separation between presentation, business logic, and data access |
| **MVC** | Presentation architectural pattern | Organizes controllers, Razor Views, models, ViewModels, routing, and model binding |
| **Service Layer** | Application pattern | Holds business rules and coordinates use cases such as checkout and stock validation |
| **Repository** | Data-access design pattern | Encapsulates EF Core queries and persistence operations behind interfaces |
| **Unit of Work** | Data-access design pattern | Coordinates `SaveChanges` and database transactions across repositories |
| **Dependency Injection** | Design technique | Supplies services and repositories through abstractions and keeps components loosely coupled |
| **ViewModels and DTOs** | Boundary-model technique | Keeps form and display data separate from persistence entities |

### Layered application flow

```text
Client / Browser
       |
       v
+--------------------------------------------------+
| Presentation Layer                               |
| ASP.NET Core MVC                                 |
| Controllers | Razor Views | ViewModels | DTOs    |
+--------------------------------------------------+
       |
       v
+--------------------------------------------------+
| Business Layer                                   |
| Application Services                             |
| Business rules | Checkout | Stock | SKU | Images |
+--------------------------------------------------+
       |
       v
+--------------------------------------------------+
| Data Access Layer                                |
| Repository Pattern | Unit of Work | EF Core      |
+--------------------------------------------------+
       |
       v
+--------------------------------------------------+
| SQL Server                                       |
+--------------------------------------------------+
```

A typical request follows this direction:

```text
HTTP Request
    -> Controller
    -> Service
    -> Repository
    -> Entity Framework Core
    -> SQL Server
```

The result returns through the same layers in reverse. Controllers do not query `ApplicationDbContext` directly, and Razor Views do not contain business or persistence logic.

### Responsibilities of each layer

- **Presentation Layer:** Handles HTTP requests, routing, model binding, validation, authorization, and rendering Razor Views.
- **Business Layer:** Applies business rules and coordinates workflows such as cart validation, checkout, stock updates, SKU generation, and image handling.
- **Data Access Layer:** Encapsulates queries and persistence through generic and entity-specific repositories.
- **Database:** SQL Server stores application and ASP.NET Core Identity data, while EF Core maps the domain model and relationships.

This structure makes responsibilities visible and keeps changes localized. A UI change should stay in the Presentation Layer, a business-rule change should stay in a service, and a query change should stay in a repository.

## What can you do in ShopHub?

### Customer experience

- Create an account and sign in securely.
- Browse a paginated product catalog.
- Search products and filter them through parent and child categories.
- View product information, price, availability, and stock status.
- Add products to the cart without reloading the page.
- Update quantities or remove items through AJAX requests.
- Keep cart data between requests using ASP.NET Core Session.
- Save and manage shipping addresses.
- Complete checkout with stock validation and database transactions.
- Review previous orders and their current status.

### Admin experience

- View store statistics, recent orders, and low-stock products from the dashboard.
- Create, edit, view, and delete products.
- Upload product images and generate unique SKUs.
- Manage parent and child categories.
- Review orders and expand them to see customer, shipping, and item information.
- Change an order's status through AJAX.
- Create administrator accounts and manage roles.
- Preview the storefront from an administrator account.

### Read-only Demo Admin

Recruiters and reviewers can explore the administration area without receiving permission to change store data.

| | Demo credentials |
|---|---|
| Email | `demo.admin@shophub.com` |
| Password | `DemoAdmin123!` |
| Access | Dashboard, products, categories, orders, and product details |

The Demo Admin cannot create, edit, or delete data, update order statuses, create administrators, or manage roles. These restrictions are enforced by server-side authorization, not only by hiding buttons.

## Technologies used

| Technology | How it is used |
|---|---|
| **C# and .NET 8** | Application logic and the ASP.NET Core runtime |
| **ASP.NET Core MVC** | Models, Razor Views, controllers, routing, Areas, model binding, and validation |
| **HTML5 and CSS3** | Page structure, responsive layouts, and custom styling |
| **Bootstrap 5** | Responsive grid, navigation, forms, cards, tables, and reusable UI components |
| **JavaScript and jQuery** | Client-side interaction and DOM updates |
| **AJAX** | Cart actions and admin order-status updates without full page reloads |
| **Razor** | Server-rendered, strongly typed views and reusable partial views |
| **Entity Framework Core 8** | Data access, relationships, LINQ queries, and Code First migrations |
| **SQL Server** | Persistent storage for users, products, categories, addresses, and orders |
| **ASP.NET Core Identity** | Registration, login, password hashing, roles, authorization, and account lockout |
| **ASP.NET Core Session** | JSON-serialized shopping-cart state kept across requests |
| **Repository and Unit of Work** | Separation of data-access logic and coordinated database changes |
| **Dependency Injection** | Connecting controllers, services, repositories, and infrastructure |

## Project structure

```text
MiniECommerce/
|-- Areas/
|   |-- Admin/              # Dashboard and store management
|   `-- Customer/           # Catalog, cart, checkout, addresses, and orders
|-- Controllers/            # Shared home and error controllers
|-- Data/                   # EF Core DbContext and entity configuration
|-- DTO/                    # Data shapes used by order views
|-- Extensions/             # Session serialization helpers
|-- Interfaces/
|   |-- Repositories/
|   `-- Services/
|-- Migrations/             # EF Core database migrations
|-- Models/                 # Domain and Identity models
|-- Repositories/           # Repository and Unit of Work implementations
|-- Services/               # Business and application services
|-- Views/                  # Shared Razor views and layouts
|-- wwwroot/                # CSS, JavaScript, libraries, and product images
`-- Program.cs              # Dependency injection, middleware, and data seeding
```

## A few implementation details

- The cart is stored as JSON in `ISession`, allowing it to survive across requests without writing unfinished carts to the database.
- Checkout validates every cart item against the latest database stock before creating an order.
- Order creation, order items, and stock changes are committed inside one database transaction.
- Order items store their purchase price so historical totals remain correct if a product price changes later.
- Categories use a self-referencing relationship to support parent and child categories.
- Products, categories, and order numbers use unique database indexes where appropriate.
- Role-based authorization separates `Customer`, `Admin`, and read-only `DemoAdmin` access.
- Anti-forgery validation protects state-changing form and AJAX requests.

## Run the project locally

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or SQL Server Express
- Visual Studio 2022, Rider, or Visual Studio Code

### 1. Clone the repository

```bash
git clone https://github.com/KRM-DevIT/MVC-Project-E-Commerce.git
cd MVC-Project-E-Commerce/MiniECommerce
```

### 2. Configure SQL Server

Update the connection string in `appsettings.json` to match your SQL Server instance:

```json
"ConnectionStrings": {
  "ConnectionString": "Server=.;Database=E-Commerce;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Configure the first full Admin account

The full administrator credentials are read from .NET user secrets and are not included in the repository:

```bash
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "YourStrongP@ssw0rd!"
```

The read-only Demo Admin account is seeded automatically.

### 4. Create the database

```bash
dotnet ef database update
```

If the Entity Framework CLI is not installed, install the .NET 8 version first:

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

### 5. Start the application

```bash
dotnet run
```

Then open `https://localhost:7059` or `http://localhost:5075`.

## Why I built this

This is a personal portfolio project, but I approached it as a real application rather than a collection of disconnected CRUD pages. The most valuable part of building ShopHub was connecting the complete flow: authentication, catalog browsing, session-based cart management, transactional checkout, inventory updates, order tracking, and role-aware administration.

The project demonstrates my experience working with server-rendered MVC applications and my understanding of how the frontend, business logic, persistence layer, and security concerns fit together in ASP.NET Core.

## License

This project is available under the [MIT License](LICENSE).
