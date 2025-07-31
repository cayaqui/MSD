# Recomendaciones para Servicios Genéricos en EzPro MSD

## 🎯 Enfoque Recomendado: Híbrido

### 1. **Usa BaseService<T> para operaciones CRUD comunes**
```csharp
public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
```

**Ventajas:**
- ✅ Implementación automática de operaciones básicas
- ✅ Manejo consistente de auditoría (CreatedBy, UpdatedBy)
- ✅ Soporte automático para soft delete
- ✅ Logging estandarizado
- ✅ Validación centralizada

### 2. **Extiende BaseService para lógica específica**
```csharp
public class UserService : BaseService<User, UserDto, CreateUserDto, UpdateUserDto>, IUserService
{
    // Métodos específicos del dominio
    Task<UserDto?> SyncWithAzureAdAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetByRoleAsync(string roleName);
}
```

### 3. **Servicios que SÍ deberían usar BaseService**
- ✅ **Catálogos simples**: Company, Operation, Discipline, Contractor
- ✅ **Entidades de configuración**: Roles, Permissions
- ✅ **Maestros básicos**: PackageType, NotificationType

### 4. **Servicios que NO deberían usar solo BaseService**
- ❌ **User**: Requiere sincronización con Azure AD
- ❌ **Project**: Lógica compleja de estado y permisos
- ❌ **Budget**: Cálculos financieros y versiones
- ❌ **Commitment/Invoice**: Workflows de aprobación
- ❌ **Notification**: Lógica de envío y expiración

## 📋 Estructura Sugerida

```
Application/
├── Interfaces/
│   ├── Base/
│   │   └── IService<T,TDto,TCreateDto,TUpdateDto>
│   ├── Security/
│   │   ├── IUserService.cs (extiende IService + métodos custom)
│   │   └── IPermissionService.cs
│   └── Setup/
│       ├── IProjectService.cs (extiende IService + métodos custom)
│       └── IDisciplineService.cs (extiende IService)
└── Services/
    ├── Base/
    │   └── BaseService<T>.cs
    ├── Security/
    │   └── UserService.cs : BaseService<User>
    └── Setup/
        ├── ProjectService.cs : BaseService<Project>
        └── DisciplineService.cs : BaseService<Discipline>
```

## 🔧 Implementación Práctica

### Para entidades simples:
```csharp
// Solo necesitas registrar el servicio base
services.AddScoped<IDisciplineService, DisciplineService>();

// DisciplineService hereda todo de BaseService
public class DisciplineService : BaseService<Discipline, DisciplineDto, CreateDisciplineDto, UpdateDisciplineDto>, IDisciplineService
{
    // Solo agregas métodos específicos si los necesitas
}
```

### Para entidades complejas:
```csharp
public class ProjectService : BaseService<Project, ProjectDto, CreateProjectDto, UpdateProjectDto>, IProjectService
{
    // Heredas CRUD básico
    
    // Agregas lógica específica
    public async Task<ProjectDto> StartProjectAsync(Guid projectId) { }
    public async Task<ProjectDto> AssignTeamMemberAsync(Guid projectId, Guid userId) { }
    public async Task<BudgetSummaryDto> GetProjectBudgetSummaryAsync(Guid projectId) { }
    
    // Sobrescribes métodos cuando necesitas lógica especial
    public override async Task<ProjectDto> CreateAsync(CreateProjectDto dto, string? createdBy)
    {
        // Validaciones especiales
        // Crear estructura WBS
        // Asignar permisos iniciales
        // etc.
    }
}
```

## ⚡ Beneficios del Enfoque Híbrido

1. **Rapidez inicial**: CRUD gratis para el 70% de las entidades
2. **Flexibilidad**: Puedes extender cuando lo necesites
3. **Consistencia**: Misma interfaz para todas las operaciones
4. **Mantenibilidad**: Cambios centralizados en BaseService
5. **Testing**: Más fácil mockear y probar

## 🚫 Evita estos errores

1. **No fuerces todo en el genérico**: Si necesitas 5+ overrides, mejor crea un servicio específico
2. **No ignores la validación**: Siempre override `ValidateEntityAsync` para reglas de negocio
3. **No olvides los includes**: Configura las propiedades de navegación necesarias
4. **No abuses de la herencia**: Máximo 1-2 niveles de herencia

## 📊 Decisión Final

Para EzPro MSD, recomiendo:
- ✅ **SÍ usar BaseService** como base
- ✅ **SÍ extender** con interfaces específicas por dominio
- ✅ **SÍ agregar** métodos específicos según necesidad
- ✅ **SÍ mantener** flexibilidad para cambios futuros