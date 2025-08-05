# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

EzPro is a Project Control System for Engineering and Construction, built with:
- .NET 9.0 following Clean Architecture principles
- Blazor WebAssembly frontend with MudBlazor UI components
- Entity Framework Core with SQL Server
- Azure AD authentication using MSAL
- Carter for minimal API endpoints

## Architecture Layers

1. **Domain** - Core business entities, interfaces, and domain logic
2. **Core** - DTOs, enums, constants, and shared value objects
3. **Application** - Business services, validators, mappings, and application logic
4. **Infrastructure** - Data persistence, external services, EF Core implementations
5. **Api** - REST API with Carter modules for endpoint organization
6. **Web** - Blazor WebAssembly frontend application

## Running the Applications

### API Backend
```bash
cd Api
dotnet run --launch-profile https
```
Runs on: https://localhost:7193

### Blazor Frontend
```bash
cd Web
dotnet run --launch-profile https
```
Runs on: https://localhost:7284

## Database Operations

### Create Migration
```bash
dotnet ef migrations add [MigrationName] -p Infrastructure -s Api
```

### Update Database
```bash
dotnet ef database update -p Infrastructure -s Api
```

## Build Commands

### Build Solution
```bash
dotnet build
```

### Run Tests
```bash
dotnet test
```

## Key Architectural Patterns

### API Organization
- Endpoints organized in Carter modules under `Api/Modules/`
- Module structure: Auth, Cost, Organization, Contracts
- Middleware pipeline: ExceptionHandling → Authentication → CurrentUser → UserValidation → Authorization

### Service Layer
- Services registered in `Application/DependencyInjection.cs`
- Scoped lifetime for most services
- Key services: ICurrentUserService, IPermissionService, IProjectService

### Data Access
- Repository pattern with IRepository<T> and IUnitOfWork
- Entity configurations in `Infrastructure/Data/Configurations/`
- Soft delete support via ISoftDelete interface

### Frontend Services
- API communication through IApiService
- Named HttpClient "EzProAPI" with authorization handler
- Services organized by domain: Auth, Cost, Organization

### Authentication Flow
- Azure AD via MSAL
- JWT Bearer tokens for API
- Permission-based authorization with project-level access control
- Custom authorization handler for system admin roles

## Development Workflow

### Adding New Features
1. Define entity in `Domain/Entities/`
2. Create DTOs in `Core/DTOs/`
3. Add service interface in `Application/Interfaces/`
4. Implement service in `Application/Services/`
5. Create Carter module in `Api/Modules/`
6. Add frontend service in `Web/Services/`
7. Create Blazor pages/components in `Web/Pages/`

### Configuration Files
- `Api/appsettings.json` - API configuration, Azure AD, connection strings
- `Web/wwwroot/appsettings.json` - Frontend API endpoint configuration
- CORS settings differ between Development (allow all) and Production (restricted)

## Important Technical Details

### Entity Framework
- Code-first approach
- Automatic audit fields (CreatedBy, ModifiedBy, etc.)
- Hierarchical entities support via IHierarchical<T>

### API Response Format
- Consistent error handling via ExceptionHandlingMiddleware
- ProblemDetails for error responses
- PagedResult<T> for paginated responses

### Frontend State Management
- Scoped services for state management
- IStateService for application state
- Blazored.LocalStorage and SessionStorage for persistence