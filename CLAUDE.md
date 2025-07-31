# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

EzPro MSD is a Project Control System for Engineering and Construction built with:
- **Backend**: ASP.NET Core 9.0 Web API with minimal APIs (Carter)
- **Frontend**: Blazor WebAssembly
- **Architecture**: Clean Architecture with DDD principles
- **Authentication**: Azure AD/Entra ID with JWT tokens
- **Database**: Entity Framework Core with SQL Server

### Project Structure

- **Api**: Web API layer with minimal API endpoints
- **Application**: Business logic, interfaces, and services
- **Domain**: Core entities and domain logic
- **Infrastructure**: Data access, external services, authentication
- **DTOs**: Data Transfer Objects for API communication
- **Enums**: Shared enumerations
- **ValueObjects**: Domain value objects
- **Web**: Blazor WebAssembly frontend

### Key Patterns

1. **Repository Pattern**: Generic repository with Unit of Work
2. **Service Layer**: BaseService<T> for CRUD operations, extended for complex logic
3. **Minimal APIs**: Using Carter for endpoint organization
4. **Authentication Flow**: Azure AD → API validation → Database check → JWT token

## Common Development Commands

### Build and Run

```bash
# Build entire solution
dotnet build

# Run API (from Api directory)
cd Api
dotnet run

# Run with specific launch profile
dotnet run --launch-profile https

# Run Blazor frontend (from Web directory)
cd Web
dotnet run
```

### Database Operations

```bash
# Apply migrations (from Api directory)
cd Api
dotnet ef database update

# Create new migration
dotnet ef migrations add MigrationName -p ../Infrastructure -s .

# Remove last migration
dotnet ef migrations remove -p ../Infrastructure -s .
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Key Configuration

### API Configuration (appsettings.json)
- Azure AD settings: `AzureAd` section
- Connection strings: `ConnectionStrings:DefaultConnection`
- JWT settings: `Authentication:Jwt`
- CORS origins: `CorsSettings:AllowedOrigins`

### Frontend Configuration (wwwroot/appsettings.json)
- API URL configuration
- Azure AD client settings

## Important Services and Interfaces

### Authentication & Security
- `ICurrentUserService`: Access authenticated user context
- `IGraphApiService`: Microsoft Graph integration
- `UserValidationMiddleware`: Validates users exist in database

### Core Services Pattern
- `IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>`: Generic CRUD operations
- Extend BaseService for domain-specific logic (e.g., UserService, ProjectService)

### Key Endpoints
- `/api/auth/*`: Authentication operations
- `/api/companies/*`: Company management
- `/api/projects/*`: Project operations
- `/api/users/*`: User management (Admin only)

## Development Workflow

1. **Adding New Entity**:
   - Create entity in Domain/Entities
   - Add DTOs (Dto, CreateDto, UpdateDto)
   - Create interface in Application/Interfaces
   - Implement service extending BaseService
   - Add configuration in Infrastructure/Data/Configurations
   - Register endpoints in Api/Endpoints

2. **Authentication Required**:
   - All endpoints require authentication by default
   - Use `[AllowAnonymous]` sparingly
   - User must exist in database (not just Azure AD)

3. **Database Context**:
   - ApplicationDbContext includes all entities
   - Configurations use Fluent API
   - Soft delete is implemented via ISoftDelete interface

## Security Considerations

- Never commit secrets or connection strings
- Use Azure Key Vault for production secrets
- All API calls require HTTPS
- JWT tokens expire after 60 minutes
- Refresh tokens valid for 7 days