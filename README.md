# ECommercePlatform

> **Note:** This project is in daily development. Features, APIs, and structure may change frequently.

A clean architecture-based e-commerce API built with ASP.NET Core (.NET 9), Entity Framework Core, JWT authentication and Angular 19.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Advanced Patterns & Libraries](#advanced-patterns--libraries)
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
- **Database:** Microsoft SQL Server

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
- Advanced pattern Mediator
- Advanced Libraries FluentValidation and CSharp Functional Extension
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
- **HTML**
- **CSS**
- **SCSS**
- **Javascript**
- **Typescript**

## Advanced Patterns & Libraries

### Mediator Pattern (MediatR)

The application implements the Mediator pattern using [MediatR](https://github.com/jbogard/MediatR) to decouple request/response handling:

- **Commands & Queries**: Following CQRS principles, operations are separated into commands (write operations) and queries (read operations)
- **Handlers**: Each command/query has a dedicated handler (e.g., `LoginHandler`, `GetCountryByIdHandler`)
- **Pipeline Behaviors**: Custom behaviors for cross-cutting concerns:
  - `ValidationBehavior`: Automatic validation using FluentValidation
  - `AuditBehavior`: Tracks entity changes for auditing
  - `TransactionBehavior`: Manages database transactions

Example usage in controllers:

```
// Sending a command
var result = await _mediator.Send(new LoginCommand { Email = "user@example.com", Password = "password" });

// Sending a query
var countries = await _mediator.Send(new GetAllCountriesQuery());
```

### FluentValidation

[FluentValidation](https://fluentvalidation.net/) provides a fluent interface for defining validation rules:

- **Validators**: Each command/query has a dedicated validator (e.g., `LoginValidator`, `CreateCountryValidator`)
- **Validation Pipeline**: Automatically validates requests before reaching handlers
- **Custom Error Messages**: Detailed, localized validation messages

Example validator:

```
public class LoginValidator : AbstractValidator<LoginCommand>
{
  public LoginValidator()
  {
    RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required.")
        .EmailAddress().WithMessage("Invalid email format.");

    RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is required.")
        .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
  }
}
```

### Result Pattern

The application uses a custom `AppResult<T>` class inspired by [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions) to handle operation results:

- **Success/Failure Results**: Explicit representation of operation outcomes
- **Error Handling**: Encapsulates error messages without exceptions
- **Fluent API**: Enables method chaining for processing results

Example usage:

```
// Returning a result
public async Task<AppResult<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
{
  try
  {
    // Process login
    var result = await _authService.LoginAsync(loginDto);
    return AppResult<AuthResultDto>.Success(result);
  }
  catch (Exception ex)
  {
    return AppResult<AuthResultDto>.Failure($"An error occurred: {ex.Message}");
  }
}

// Handling a result
var result = await _mediator.Send(new LoginCommand { Email = email, Password = password });
if (result.IsSuccess)
  return Ok(result.Value); else return BadRequest(new { message = result.Error });
```
  
## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Angular CLI](https://angular.dev/tools/cli) (`npm install -g @angular/cli`)
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

Migrations are applied automatically on startup. 

- To add new migrations manually:

  - Go to `Tools > NuGet Package Manager > Package Manger Console`

  - Set `ECommercePlatform.Infrastructure` as your Default project.

- Run below Commands:

  To add new migration

  - PM>```Add Migration "Migration Name"```

  To update database

  - PM>```Update Database```

## API Documentation

Swagger UI is enabled in development mode.

- Visit: `https://localhost:5001/swagger`
- Use the "Authorize" button to authenticate with a JWT token.

## Project Structure

```
ECommercePlatform
├── ECommercePlatform.API
│   ├── Controllers
│   │   ├── AuthController.cs
│   │   ├── CityController.cs
│   │   ├── CountryController.cs
│   │   ├── ProductController.cs
│   │   └── StateController.cs
│   ├── Middleware
│   │   ├── ExceptionMiddleware.cs
│   │   ├── MiddlewareExtensions.cs
│   │   └── ValidationMiddleware.cs
├── ECommercePlatform.Application
│   ├── Common
│   │   ├── Behaviors
│   │   │   ├── AuditBehavior.cs
│   │   │   ├── TransactionBehavior.cs
│   │   │   └── ValidationBehavior.cs
│   │   ├── Interfaces
│   │   │   ├── IAuditableCreateRequest.cs
│   │   │   ├── IAuditableUpdateRequest.cs
│   │   │   └── ITransactionalBehavior.cs
│   │   ├── Models
│   │   │   └── Result.cs
│   │   └── DependencyInjection.cs
│   ├── DTOs
│   │   ├── AuthDto.cs
│   │   ├── CityDto.cs
│   │   ├── CountryDto.cs
│   │   ├── ProductDto.cs
│   │   └── StateDto.cs
│   ├── Features
│   │   ├── Auth
│   │   │   ├── Commands
│   │   │   │   ├── Login
│   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   ├── LoginHandler.cs
│   │   │   │   │   └── LoginValidator.cs
│   │   │   │   └── Register
│   │   │   │       ├── RegisterCommand.cs
│   │   │   │       ├── RegisterHandler.cs
│   │   │   │       └── RegisterValidator.cs
│   │   │   └── Queries
│   │   │       └── GetCurrentUser
│   │   │           ├── GetCurrentUserHandler.cs
│   │   │           ├── GetCurrentUserQuery.cs
│   │   │           └── GetCurrentUserValidator.cs
│   │   ├── Cities
│   │   │   ├── Commands
│   │   │   │   ├── Create
│   │   │   │   │   ├── CreateCityCommand.cs
│   │   │   │   │   ├── CreateCityHandler.cs
│   │   │   │   │   └── CreateCityValidator.cs
│   │   │   │   ├── Delete
│   │   │   │   │   ├── DeleteCityCommand.cs
│   │   │   │   │   ├── DeleteCityHandler.cs
│   │   │   │   │   └── DeleteCityValidator.cs
│   │   │   │   └── Update
│   │   │   │       ├── UpdateCityCommand.cs
│   │   │   │       ├── UpdateCityHandler.cs
│   │   │   │       └── UpdateCityValidator.cs
│   │   │   └── Queries
│   │   │       ├── GetAllCities
│   │   │       │   ├── GetAllCitiesHandler.cs
│   │   │       │   ├── GetAllCitiesQuery.cs
│   │   │       │   └── GetAllCitiesValidator.cs
│   │   │       ├── GetCitiesByState
│   │   │       │   ├── GetCitiesByStateQuery.cs
│   │   │       │   ├── GetCitysByStateHandler.cs
│   │   │       │   └── GetCitysByStateValidator.cs
│   │   │       └── GetCityById
│   │   │           ├── GetCityByIdHandler.cs
│   │   │           ├── GetCityByIdQuery.cs
│   │   │           └── GetCityByIdValidator.cs
│   │   ├── Countries
│   │   │   ├── Commands
│   │   │   │   ├── Create
│   │   │   │   │   ├── CreateCountryCommand.cs
│   │   │   │   │   ├── CreateCountryHandler.cs
│   │   │   │   │   └── CreateCountryValidator.cs
│   │   │   │   ├── Delete
│   │   │   │   │   ├── DeleteCountryCommand.cs
│   │   │   │   │   ├── DeleteCountryHandler.cs
│   │   │   │   │   └── DeleteCountryValidator.cs
│   │   │   │   └── Update
│   │   │   │       ├── UpdateCountryCommand.cs
│   │   │   │       ├── UpdateCountryHandler.cs
│   │   │   │       └── UpdateCountryValidator.cs
│   │   │   └── Queries
│   │   │       ├── GetAllCountries
│   │   │       │   ├── GetAllCountriesHandler.cs
│   │   │       │   ├── GetAllCountriesQuery.cs
│   │   │       │   └── GetAllCountriesValidator.cs
│   │   │       └── GetCountryById
│   │   │           ├── GetCountryByIdHandler.cs
│   │   │           ├── GetCountryByIdQuery.cs
│   │   │           └── GetCountryByIdValidator.cs
│   │   ├── Products
│   │   │   ├── Commands
│   │   │   │   ├── Handlers
│   │   │   │   │   └── CreateProductHandler.cs
│   │   │   │   ├── Validators
│   │   │   │   │   └── CreateProductValidator.cs
│   │   │   │   └── CreateProductCommand.cs
│   │   │   └── Queries
│   │   │       ├── Handlers
│   │   │       │   └── GetProductByIdHandler.cs
│   │   │       ├── Validators
│   │   │       │   └── GetProductByIdValidator.cs
│   │   │       └── GetProductByIdQuery.cs
│   │   └── States
│   │       ├── Commands
│   │       │   ├── Create
│   │       │   │   ├── CreateStateCommand.cs
│   │       │   │   ├── CreateStateHandler.cs
│   │       │   │   └── CreateStateValidator.cs
│   │       │   ├── Delete
│   │       │   │   ├── DeleteStateCommand.cs
│   │       │   │   ├── DeleteStateHandler.cs
│   │       │   │   └── DeleteStateValidator.cs
│   │       │   └── Update
│   │       │       ├── UpdateStateCommand.cs
│   │       │       ├── UpdateStateHandler.cs
│   │       │       └── UpdateStateValidator.cs
│   │       └── Queries
│   │           ├── GetAllStates
│   │           │   ├── GetAllStatesHandler.cs
│   │           │   ├── GetAllStatesQuery.cs
│   │           │   └── GetAllStatesValidator.cs
│   │           ├── GetStatesByCountry
│   │           │   ├── GetStatesByCountryHandler.cs
│   │           │   ├── GetStatesByCountryQuery.cs
│   │           │   └── GetStatesByCountryValidator.cs
│   │           └── GetStatesById
│   │               ├── GetStateByIdHandler.cs
│   │               ├── GetStateByIdQuery.cs
│   │               └── GetStateByIdValidator.cs
│   ├── Interfaces
│   │   ├── ICity
│   │   │   ├── ICityRepository.cs
│   │   │   └── ICityService.cs
│   │   ├── ICountry
│   │   │   ├── ICountryRepository.cs
│   │   │   └── ICountryService.cs
│   │   ├── IGeneral
│   │   │   └── IGenericRepository.cs
│   │   ├── IProduct
│   │   │   ├── IProductRepository.cs
│   │   │   └── IProductService.cs
│   │   ├── IState
│   │   │   ├── IStateRepository.cs
│   │   │   └── IStateService.cs
│   │   ├── IUserAuth
│   │   │   ├── IAuthService.cs
│   │   │   ├── ICurrentUserService.cs
│   │   │   └── IUserRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Mappings
│   │   └── MappingProfile.cs
│   ├── Services
│   │   ├── AuthService.cs
│   │   ├── CityService.cs
│   │   ├── CountryService.cs
│   │   ├── CurrentUserService.cs
│   │   ├── ProductService.cs
│   │   ├── StateService.cs
│   │   └── ValidationService.cs
│   └── ECommercePlatform.Application.csproj
├── ECommercePlatform.client
│   ├── public
│   │   ├── cities.png
│   │   ├── countries.png
│   │   ├── dashboard.png
│   │   ├── favicon.ico
│   │   ├── login.png
│   │   └── states.png
│   ├── src
│   │   ├── app
│   │   │   ├── admin
│   │   │   │   ├── city
│   │   │   │   │   ├── city.component.html
│   │   │   │   │   ├── city.component.scss
│   │   │   │   │   ├── city.component.spec.ts
│   │   │   │   │   └── city.component.ts
│   │   │   │   ├── country
│   │   │   │   │   ├── country.component.html
│   │   │   │   │   ├── country.component.scss
│   │   │   │   │   ├── country.component.spec.ts
│   │   │   │   │   └── country.component.ts
│   │   │   │   ├── dashboard
│   │   │   │   │   ├── dashboard.component.html
│   │   │   │   │   ├── dashboard.component.scss
│   │   │   │   │   ├── dashboard.component.spec.ts
│   │   │   │   │   └── dashboard.component.ts
│   │   │   │   ├── login
│   │   │   │   │   ├── login.component.html
│   │   │   │   │   ├── login.component.scss
│   │   │   │   │   ├── login.component.spec.ts
│   │   │   │   │   └── login.component.ts
│   │   │   │   ├── navbar
│   │   │   │   │   ├── navbar.component.html
│   │   │   │   │   ├── navbar.component.scss
│   │   │   │   │   ├── navbar.component.spec.ts
│   │   │   │   │   └── navbar.component.ts
│   │   │   │   ├── state
│   │   │   │   │   ├── state.component.html
│   │   │   │   │   ├── state.component.scss
│   │   │   │   │   ├── state.component.spec.ts
│   │   │   │   │   └── state.component.ts
│   │   │   │   ├── admin-routing.module.ts
│   │   │   │   └── admin.module.ts
│   │   │   ├── guards
│   │   │   │   └── auth.guard.ts
│   │   │   ├── models
│   │   │   │   ├── city.model.ts
│   │   │   │   ├── country.model.ts
│   │   │   │   ├── state.model.ts
│   │   │   │   └── user.model.ts
│   │   │   ├── page-not-found
│   │   │   │   ├── page-not-found.component.html
│   │   │   │   ├── page-not-found.component.scss
│   │   │   │   ├── page-not-found.component.spec.ts
│   │   │   │   └── page-not-found.component.ts
│   │   │   ├── services
│   │   │   │   ├── auth
│   │   │   │   │   ├── auth.interceptor.ts
│   │   │   │   │   ├── auth.service.spec.ts
│   │   │   │   │   └── auth.service.ts
│   │   │   │   ├── city
│   │   │   │   │   ├── city.service.spec.ts
│   │   │   │   │   └── city.service.ts
│   │   │   │   ├── country
│   │   │   │   │   ├── country.service.spec.ts
│   │   │   │   │   └── country.service.ts
│   │   │   │   ├── custom-validators
│   │   │   │   │   ├── custom-validators.service.spec.ts
│   │   │   │   │   └── custom-validators.service.ts
│   │   │   │   ├── general
│   │   │   │   │   ├── favicon.service.ts
│   │   │   │   │   ├── message.service.ts
│   │   │   │   │   └── theme.service.ts
│   │   │   │   ├── state
│   │   │   │   │   ├── state.service.spec.ts
│   │   │   │   │   └── state.service.ts
│   │   │   │   ├── no-white-space.service.spec.ts
│   │   │   │   └── no-white-space.service.ts
│   │   │   ├── shared
│   │   │   │   └── message
│   │   │   │       ├── message.component.css
│   │   │   │       ├── message.component.html
│   │   │   │       ├── message.component.spec.ts
│   │   │   │       └── message.component.ts
│   │   │   ├── app.component.html
│   │   │   ├── app.component.scss
│   │   │   ├── app.component.spec.ts
│   │   │   ├── app.component.ts
│   │   │   ├── app.config.ts
│   │   │   └── app.routes.ts
│   │   ├── environments
│   │   │   ├── environment.prod.ts
│   │   │   └── environment.ts
│   │   ├── index.html
│   │   ├── main.ts
│   │   ├── proxy.conf.js
│   │   ├── script.js
│   │   ├── styles.css
│   │   └── THEME_DOCUMENTATION.md
│   ├── .editorconfig
│   ├── .gitignore
│   ├── angular.json
│   ├── aspnetcore-https.js
│   ├── CHANGELOG.md
│   ├── ecommerceplatform.client.esproj
│   ├── ECommercePlatform.client.esproj.Backup.tmp
│   ├── ecommerceplatform.client.esproj.user
│   ├── karma.conf.js
│   ├── package-lock.json
│   ├── package-lock.json~RF5ec1203.TMP
│   ├── package.json
│   ├── packages.config
│   ├── README.md
│   ├── tsconfig.app.json
│   ├── tsconfig.json
│   └── tsconfig.spec.json
├── ECommercePlatform.Domain
│   ├── Entities
│   │   ├── Address.cs
│   │   ├── BaseEntity.cs
│   │   ├── Category.cs
│   │   ├── City.cs
│   │   ├── Country.cs
│   │   ├── Coupon.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Product.cs
│   │   ├── ProductVariant.cs
│   │   ├── Review.cs
│   │   ├── ShippingAddress.cs
│   │   ├── State.cs
│   │   └── User.cs
│   ├── Enums
│   │   └── Enums.cs
│   ├── Exceptions
│   │   └── DuplicateResourceException.cs
├── ECommercePlatform.Infrastructure
│   ├── bin
│   │   └── Debug
│   │       └── net9.0
│   │           ├── ECommercePlatform.Application.dll
│   │           ├── ECommercePlatform.Application.pdb
│   │           ├── ECommercePlatform.Domain.dll
│   │           ├── ECommercePlatform.Domain.pdb
│   │           ├── ECommercePlatform.Infrastructure.deps.json
│   │           ├── ECommercePlatform.Infrastructure.dll
│   │           ├── ECommercePlatform.Infrastructure.pdb
│   │           └── ECommercePlatform.Infrastructure.runtimeconfig.json
│   ├── Migrations
│   │   ├── AppDb
│   │   │   ├── 20250512054037_CleanArchitecture.cs
│   │   │   └── 20250512054037_CleanArchitecture.Designer.cs
│   │   ├── 20250502092659_InitialCreate.cs
│   │   ├── 20250502092659_InitialCreate.Designer.cs
│   │   ├── 20250502095235_UpdateSeedData.cs
│   │   ├── 20250502095235_UpdateSeedData.Designer.cs
│   │   ├── 20250506063456_GeneralChanges.cs
│   │   ├── 20250506063456_GeneralChanges.Designer.cs
│   │   └── AppDbContextModelSnapshot.cs
│   ├── Repositories
│   │   ├── CityRepository.cs
│   │   ├── CountryRepository.cs
│   │   ├── GenericRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── StateRepository.cs
│   │   └── UserRepository.cs
│   ├── AppDbContext.cs
│   ├── ECommercePlatform.Infrastructure.csproj
│   └── UnitOfWork.cs
├── ECommercePlatform.Server
│   ├── Properties
│   │   └── launchSettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── CHANGELOG.md
│   ├── Program.cs
│   ├── README.md
│   └── WeatherForecast.cs
├── ECommercePlatform.sln
├── ECommercePlatform.slnLaunch.user
├── LICENSE.txt
└── README.md
```

- **API:** Entry point, middleware, controllers.
- **Application:** Business logic, service interfaces, DTOs, mappings.
- **Infrastructure:** Data access, repositories, EF Core context.
- **Domain:** Entities, Enums, Exceptions.

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE.txt) file for details.
