# ECommerce Platform - Clean Architecture

This project implements a .NET Core 9.0 Web API for an eCommerce platform, following Clean Architecture principles.

## Architecture Overview

The solution is organized into four main layers:

### 1. Domain Layer (Core/Domain)
- Contains all entities, enums, and domain logic
- Has no dependencies on other layers or external frameworks
- Example: `User`, `Product`, `Order`, etc.

### 2. Application Layer (Core/Application)
- Contains business logic and orchestration
- Depends only on the Domain layer
- Implements interfaces for external services
- Includes:
  - DTOs for data transfer
  - Interface definitions for repositories and services
  - Service implementations with business logic

### 3. Infrastructure Layer (Infrastructure)
- Contains external concerns and implementation details
- Depends on the Application layer to implement its interfaces
- Includes:
  - Database context and migrations
  - Repository implementations
  - External service implementations
  - Identity services

### 4. Presentation Layer (Presentation)
- Contains API controllers and middleware
- Depends on the Application layer to consume its services
- Includes:
  - API endpoints
  - Request/response models
  - Authentication middleware
  - Validation middleware
  - Exception handling

## Key Design Patterns

- **Repository Pattern**: Abstracts data access logic
- **Unit of Work**: Manages transactions across repositories
- **Dependency Injection**: Resolves dependencies at runtime
- **Mediator Pattern**: Decouples request/response handling (for future implementation)

## Running the Application

1. Ensure you have .NET 9.0 installed
2. Update the connection string in `appsettings.json`
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

4. Access Swagger documentation at: https://localhost:5001/swagger

## Authentication

The API uses JWT Bearer token authentication. To authenticate:

1. POST to `/api/auth/login` with email and password
2. Include the returned token in the Authorization header as: `Bearer {token}`

## Project Structure

```
src/
├── Core/
│   ├── Domain/
│   │   ├── Entities/
│   │   └── Enums/
│   └── Application/
│       ├── DTOs/
│       ├── Interfaces/
│       └── Services/
├── Infrastructure/
│   ├── Persistence/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   └── Migrations/
│   └── Services/
└── Presentation/
    ├── Controllers/
    └── Middleware/
``` 