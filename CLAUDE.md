# CLAUDE.md

Este archivo proporciona orientación a Claude Code (claude.ai/code) cuando trabaje con el código en este repositorio.

## Resumen del Proyecto

EzPro es un Sistema de Control de Proyectos para Ingeniería y Construcción, construido con:
- .NET 9.0 siguiendo principios de Clean Architecture
- Frontend Blazor WebAssembly con componentes UI MudBlazor
- Entity Framework Core con SQL Server
- Autenticación Azure AD usando MSAL
- Carter para endpoints de API minimalistas

## Capas de Arquitectura

1. **Domain** - Entidades de negocio principales, interfaces y lógica del dominio
2. **Core** - DTOs, enums, constantes y objetos de valor compartidos
3. **Application** - Servicios de negocio, validadores, mapeos y lógica de aplicación
4. **Infrastructure** - Persistencia de datos, servicios externos, implementaciones de EF Core
5. **Api** - API REST con módulos Carter para organización de endpoints
6. **Web** - Aplicación frontend Blazor WebAssembly

## Ejecutar las Aplicaciones

### Backend API
```bash
cd Api
dotnet run --launch-profile https
```
Se ejecuta en: https://localhost:7193

### Frontend Blazor
```bash
cd Web
dotnet run --launch-profile https
```
Se ejecuta en: https://localhost:7284

## Operaciones de Base de Datos

### Crear Migración
```bash
dotnet ef migrations add [NombreMigracion] -p Infrastructure -s Api
```

### Actualizar Base de Datos
```bash
dotnet ef database update -p Infrastructure -s Api
```

## Comandos de Compilación

### Compilar Solución
```bash
dotnet build
```

### Ejecutar Pruebas
```bash
dotnet test
```

## Patrones Arquitectónicos Clave

### Organización de la API
- Endpoints organizados en módulos Carter bajo `Api/Modules/`
- Estructura de módulos: Auth, Cost, Organization, Contracts, Projects
- Pipeline de middleware: ExceptionHandling → Authentication → CurrentUser → UserValidation → Authorization

### Capa de Servicios
- Servicios registrados en `Application/DependencyInjection.cs`
- Tiempo de vida Scoped para la mayoría de servicios
- Servicios clave: ICurrentUserService, IPermissionService, IProjectService, IWBSService

### Acceso a Datos
- Patrón Repository con IRepository<T> e IUnitOfWork
- Configuraciones de entidades en `Infrastructure/Data/Configurations/`
- Soporte para eliminación lógica vía interfaz ISoftDelete

### Servicios Frontend
- Comunicación API a través de IApiService
- HttpClient nombrado "EzProAPI" con handler de autorización
- Servicios organizados por dominio: Auth, Cost, Organization, Projects

### Flujo de Autenticación
- Azure AD vía MSAL
- Tokens JWT Bearer para API
- Autorización basada en permisos con control de acceso a nivel de proyecto
- Handler de autorización personalizado para roles de administrador del sistema

## Flujo de Trabajo de Desarrollo

### Agregar Nuevas Características
1. Definir entidad en `Domain/Entities/`
2. Crear DTOs en `Core/DTOs/`
3. Agregar interfaz de servicio en `Application/Interfaces/`
4. Implementar servicio en `Application/Services/`
5. Crear módulo Carter en `Api/Modules/`
6. Agregar servicio frontend en `Web/Services/`
7. Crear páginas/componentes Blazor en `Web/Pages/`

### Archivos de Configuración
- `Api/appsettings.json` - Configuración de API, Azure AD, cadenas de conexión
- `Web/wwwroot/appsettings.json` - Configuración del endpoint de API del frontend
- Configuración CORS difiere entre Desarrollo (permitir todo) y Producción (restringido)

## Detalles Técnicos Importantes

### Entity Framework
- Enfoque code-first
- Campos de auditoría automáticos (CreatedBy, ModifiedBy, etc.)
- Soporte de entidades jerárquicas vía IHierarchical<T>

### Formato de Respuesta de API
- Manejo de errores consistente vía ExceptionHandlingMiddleware
- ProblemDetails para respuestas de error
- PagedResult<T> para respuestas paginadas

### Gestión de Estado Frontend
- Servicios con ámbito para gestión de estado
- IStateService para estado de aplicación
- Blazored.LocalStorage y SessionStorage para persistencia

## Idioma y Mensajes

### IMPORTANTE: Idioma Español
- **Toda la documentación del código debe estar en español**
- **Los mensajes de error deben estar en español**
- **Los mensajes de validación deben estar en español**
- **Los comentarios en el código deben estar en español**
- **Los mensajes de usuario en la interfaz deben estar en español**

### Convenciones de Mensajes
- Mensajes de error: Claros y específicos sobre el problema
- Mensajes de éxito: Confirmar la acción completada
- Mensajes de validación: Indicar qué campo y qué regla no se cumple
- Mensajes de confirmación: Preguntar claramente la acción a realizar

### Ejemplos de Mensajes
```csharp
// Mensajes de error
"Error al cargar los datos del proyecto"
"No se pudo conectar con el servidor"
"El usuario no tiene permisos para realizar esta acción"

// Mensajes de validación
"El campo nombre es requerido"
"El presupuesto debe ser mayor a cero"
"La fecha de fin debe ser posterior a la fecha de inicio"

// Mensajes de éxito
"Proyecto creado exitosamente"
"Cambios guardados correctamente"
"Paquete de trabajo actualizado"

// Mensajes de confirmación
"¿Está seguro que desea eliminar este elemento?"
"Esta acción no se puede deshacer. ¿Desea continuar?"
```