Live Demo: http://shopthehub.runasp.net/

# ShopHub - ASP.NET Core MVC E-Commerce

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-8.0-6C3483)
![SQL Server](https://img.shields.io/badge/Database-SQL_Server-CC2927?logo=microsoftsqlserver)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?logo=bootstrap)
![License](https://img.shields.io/badge/License-MIT-green)

ShopHub is a full-stack e-commerce web application I built with ASP.NET Core MVC. It covers the complete shopping flow, from browsing products and managing a cart to checkout and order tracking, together with an administration area for managing the store.

I created this project to apply the MVC pattern in a realistic application and gain hands-on experience with authentication, role-based authorization, session state, AJAX requests, database relationships, and layered application design.

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

## How the application is organized

ShopHub uses ASP.NET Core Areas to keep the customer and administration experiences separate while sharing the same services and database.

```text
Browser
  |
  |  Razor Views + HTML + CSS + Bootstrap + JavaScript/AJAX
  v
MVC Controllers
  |
  |  ViewModels and DTOs
  v
Service Layer
  |
  |  Business rules and application workflows
  v
Repositories + Unit of Work
  |
  |  Entity Framework Core
  v
SQL Server
```

- **Models** represent the main business entities: products, categories, users, addresses, orders, and order items.
- **Views** render the storefront and admin interface using Razor, HTML, CSS, and Bootstrap.
- **Controllers** receive HTTP requests, validate input, and delegate work to services.
- **Services** contain business rules such as cart validation, SKU generation, checkout, stock management, and image handling.
- **Repositories** contain database queries, while the Unit of Work coordinates saving and transactions.

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
