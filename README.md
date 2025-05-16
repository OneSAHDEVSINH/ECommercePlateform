# ECommercePlatform

> **Note:** This project is in daily development. Features, APIs, and structure may change frequently.

A clean architecture-based e-commerce API built with ASP.NET Core (.NET 9), Entity Framework Core, JWT authentication and Angular 19.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Database Migrations](#database-migrations)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

ECommercePlatform is a full-stack, clean architecture-based e-commerce solution.  
- **Backend:** ASP.NET Core (.NET 9) Web API  
- **Frontend:** Angular (located in the `client` project)  
It provides a modular, scalable foundation for building e-commerce solutions, featuring JWT authentication, role-based authorization, and robust API documentation.

## Features

- Clean architecture with separation of concerns
- JWT authentication and authorization
- Entity Framework Core with SQL Server
- AutoMapper for object mapping
- Swagger/OpenAPI documentation
- Custom middleware for exception and validation handling
- CORS support
- Automated database migrations at startup
- **Angular 19 frontend (ClientApp)**
  
## Tech Stack

*Backend*
- **.NET 9** (C# 13)
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQL Server**
- **AutoMapper**
- **Swagger/OpenAPI**
- **JWT Bearer Authentication**

*Frontend:*
- **Angular 19**
- **Javascript**
- **Typescript**
- **HTML**
- **CSS**
- **SCSS**
  
## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Angular CLI](https://angular.io/cli) (`npm install -g @angular/cli`)
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Installation

1. **Clone the repository:**
   git clone https://github.com/OneSAHDEVSINH/ECommercePlatform.git cd ECommercePlatform

2. **Configure the database connection:**
   
   - Update the `DefaultConnection` string in `appsettings.json`:
     ```
      "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ECommerceDb;Trusted_Connection=True;"
     }
     ```
 
   - Set JWT settings in `appsettings.json`:
     ```
      "Jwt": {
         "Key": "YourSuperSecretKey",
         "Issuer": "YourIssuer",
         "Audience": "YourAudience"
     }
     ```
 

4. **Restore dependencies:**
   dotnet restore

5. **Run the backend:**
   dotnet run --project ECommercePlatform.Server

   The API will be available at `https://localhost:5001` (or as configured).

### Frontend Setup (Angular)

1. **Navigate to the Angular app:**
    `cd ClientApp`

2. **Install dependencies:**
    `npm install`

3. **Run the Angular development server:**
   `ng serve`

    The frontend will be available at `http://localhost:4200` by default.

4. **API Proxy (Development):**
    - The backend is configured to proxy API requests to the Angular dev server during development (see commented SPA middleware in `Program.cs`).
    - For production, build Angular (`ng build --configuration production`) and serve the static files from ASP.NET Core's `wwwroot`.

## Configuration

- **CORS:** Configured to allow all origins, methods, and headers.
- **JWT Authentication:** Set up in `Program.cs` using settings from `appsettings.json`.
- **Database:** Uses SQL Server; migrations are applied automatically at startup.

## Database Migrations

Migrations are applied automatically on startup. To add new migrations manually:

dotnet ef migrations add MigrationName --project ECommercePlatform.Infrastructure dotnet ef database update --project ECommercePlatform.Infrastructure

## API Documentation

Swagger UI is enabled in development mode.

- Visit: `https://localhost:5001/swagger`
- Use the "Authorize" button to authenticate with a JWT token.

## Project Structure

```
ECommercePlatform
ECommercePlatform.API
│
├── Controllers/
├── Middleware/
│
ECommercePlatform.Application
│
├── DTOs/
├── Interfaces/
├── Mappings/
├── Services/
│
ECommercePlatform.client
│
├── Whole Angular frontend Project
│
ECommercePlatform.Domain
│
├── Entities/
├── Enums/
├── Exceptions/
│
EommercePlateform.Infrastructure
│
├── Migrations/
├── Repositories/
├── AppDbContext.cs
├── UnitOfWork.cs
│
ECommercePlatform.Server
│
└── (Whole Server Project)
```

- **API:** Entry point, middleware, controllers.
- **Application:** Business logic, service interfaces, DTOs, mappings.
- **Infrastructure:** Data access, repositories, EF Core context.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements.

## License

This project is licensed under the MIT License.
