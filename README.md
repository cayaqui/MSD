# Sistema de Control de Proyectos MSD - Instrucciones de Desarrollo

## 📋 Resumen Ejecutivo

Desarrollo de un API REST en .NET 9 para Sistema de Control de Proyectos de ingeniería y construcción, alineado con los estándares PMBOK®/PMI y las mejores prácticas de AACE International. El sistema utiliza Blazor WebAssembly Standalone para el frontend con diseño basado en YNEX.

## 🎯 Objetivo Principal

Crear un sistema robusto y escalable que permita:

- Control integral de proyectos según metodología PMI
- Gestión de costos siguiendo estándares AACE
- Implementación de Earned Value Management (EVM)
- Control de cuentas (Control Accounts) según documento ControlAccounts.md
- Interfaz moderna y atractiva basada en patrones YNEX

## 🏗️ Arquitectura de la Solución

### Estructura de Proyectos

```
MSD/
├── Domain/           # Entidades y lógica de dominio
├── Application/      # Servicios y lógica de aplicación
├── Infrastructure/   # Implementación de repositorios y servicios externos
├── API/             # Minimal API con Carter
├── Core/            # DTOs y contratos compartidos
└── Web/             # Blazor WebAssembly Standalone
```

### Stack Tecnológico

- **Backend**: .NET 9, Minimal API con Carter
- **Frontend**: Blazor WebAssembly Standalone
- **Base de Datos**: Azure SQL con Entity Framework Core 9
- **Autenticación**: Microsoft Entra ID (Azure AD)
- **Almacenamiento**: Azure Blob Storage
- **Monitoreo**: Application Insights
- **Validación**: FluentValidation
- **Patrones**: Repository, Unit of Work, CQRS

## 🔐 Módulo de Autenticación

### Configuración en Blazor WASM (Program.cs)

```csharp
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://your-api-id/access_as_user");
    options.ProviderOptions.LoginMode = "redirect";
});
```

### Configuración en API

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
```

## 📊 Estructura de Control Accounts (Basado en PMI)

### Formato de Codificación

```
C-XXX-YY-CAM-##
│  │   │   │   └─ Número secuencial
│  │   │   └───── Responsable del Control Account (3 letras)
│  │   └───────── Fase del proyecto
│  └───────────── Proyecto
└──────────────── Identificador Control Account
```

### Componentes del Control Account

- **Work Packages**: Nivel más bajo del WBS
- **Planning Packages**: Trabajo futuro planificado
- **BAC**: Budget at Completion (Presupuesto autorizado)
- **Método de Medición**: Forma de medir el avance
- **CAM**: Control Account Manager (Responsable único)

### Cuentas EVM Principales

```
E-PV-XXX: Planned Value (BCWS)
E-EV-XXX: Earned Value (BCWP)
E-AC-XXX: Actual Cost (ACWP)
E-CV-XXX: Cost Variance
E-SV-XXX: Schedule Variance
E-CPI-XXX: Cost Performance Index
E-SPI-XXX: Schedule Performance Index
```

## 🔧 Módulos Core del Sistema

### 1. Gestión de Configuración

- **Empresas/Organizaciones**: Multi-tenant support para autenticación, orientado a compañía única que administra muchos proyectos
- **WBS Templates**: Plantillas reutilizables
- **Roles y Permisos**: Simplificar lo más posible. El usuario accede a proyectos en base a roles definidos por proyecto. Si el usuario es Admin o es Support, tiene acceso a toda la aplicación.
- **Parámetros del Sistema**: Configuración global

### 2. Gestión de Proyectos

- **Charter del Proyecto**: Documento de inicio
- **Definición de Fases**: Según ciclo de vida
- **Stakeholders**: Registro y comunicaciones
- **Documentación**: Control de versiones

### 3. Gestión de Alcance

- **WBS (Work Breakdown Structure)**
  - Estructura jerárquica de entregables
  - Diccionario del WBS
  - Control de cambios
- **Entregables**: Definición y seguimiento
- **Baseline de Alcance**: Línea base aprobada

### 4. Gestión de Cronograma

- **Actividades**: Definición y secuenciación
- **Dependencias**: FS, SS, FF, SF
- **Ruta Crítica (CPM)**: Cálculo automático
- **Curvas S**: Visualización de avance
- **Cronograma Maestro**: Integración de actividades

### 5. Gestión de Costos (AACE Standards)

- **CBS (Cost Breakdown Structure)**
- **Clases de Estimación** (Clase 5 a Clase 1)
  - Clase 5: ±50% precisión
  - Clase 1: ±10% precisión
- **EVM (Earned Value Management)**
  - PV, EV, AC
  - CPI, SPI, TCPI
  - EAC, ETC, VAC
- **Control de Cambios**: Proceso formal
- **Forecasting**: Proyecciones actualizadas

### 6. Gestión de Contratos

- **Registro de Contratos**: Base de datos centralizada
- **Contratistas**: Evaluación y seguimiento
- **Órdenes de Cambio**: Control y aprobación
- **Valuaciones**: Certificación de avances
- **Retenciones y Garantías**: Gestión financiera

### 7. Gestión de Riesgos

- **Registro de Riesgos**: Identificación continua
- **Análisis Cualitativo**: Probabilidad e Impacto
- **Análisis Cuantitativo**: Monte Carlo, Árbol de decisiones
- **Planes de Respuesta**: Mitigar, Transferir, Aceptar, Evitar
- **Monitoreo**: Seguimiento de triggers

### 8. Gestión Documental

- **Repositorio**: Azure Blob Storage
- **Control de Versiones**: Trazabilidad completa
- **Transmittals**: Envío formal de documentos
- **Distribución**: Matriz de comunicación

### 9. Reportes y Dashboards

- **Dashboard Ejecutivo**: KPIs en tiempo real
- **Reportes de Avance**: Físico y financiero
- **Análisis de Tendencias**: Proyecciones
- **Reportes Personalizados**: Power BI integration

## 💻 Implementación Técnica

## 🎨 Frontend - Blazor WebAssembly

### Estructura de Carpetas

```
MSD.Web/
├── wwwroot/
│   ├── css/
│   │   ├── ynex-styles.min.css    # Estilos YNEX
│   │   └── app.css                # Estilos personalizados
├── Components/
│   ├── Layout/
│   │   ├── Sidebar.razor
│   │   └── Header.razor
│   ├── Shared/
│   │   ├── LoadingSpinner.razor
│   │   └── ErrorBoundary.razor
├── Layout/
├── Pages/
│   ├── Projects/
│   ├── ControlAccounts/
│   ├── Budget/
│   └── Reports/
├── Services/
│   ├── Implementation/
│   │   └── CostService.cs
│   └── Interfaces/
└── Program.cs
```

### Patrones de Diseño YNEX

- **Colores**: Variables CSS personalizables
- **Componentes**: Cards, Forms, Tables, Charts
- **Iconos**: Fontawesome Pro Light
- **Animaciones**: Transiciones suaves
- **Responsive**: Mobile-first approach
- **Theme**: Ynex / CasptoneCopper

## 📝 Plan de Desarrollo Paso a Paso

### Fase 1: Configuración Base (Sprint 1)

1. **Crear solución y proyectos**
   
   - Estructura de carpetas según arquitectura propuesta
   - Configurar referencias entre proyectos
   - Instalar paquetes NuGet necesarios

2. **Configurar Entity Framework Core**
   
   - Definir DbContext
   - Crear configuraciones de entidades
   - Implementar migraciones iniciales

3. **Implementar autenticación Azure AD**
   
   - Configurar MSAL en Blazor
   - Consumir API con mismo token y roles definidos en la base de datos.

### Fase 2: Módulos Core (Sprint 2-3)

1. **Implementar Control Accounts**
   
   - CRUD completo
   - Asignación de responsables
   - Cálculos EVM

2. **Implementar WBS**
   
   - Estructura jerárquica
   - Work Packages
   - Planning Packages

3. **Implementar Gestión de Costos**
   
   - CBS structure
   - Cost items
   - Commitments

### Fase 3: Frontend (Sprint 4-5)

1. **Configurar Blazor WebAssembly**
   
   - Integrar estilos YNEX
   - Crear layout principal
   - Implementar navegación

2. **Crear páginas principales**
   
   - Dashboard ejecutivo
   - Listado de proyectos
   - Control Accounts
   - Reportes EVM

### Fase 4: Integración y Testing (Sprint 6)

1. **Integración completa**
   
   - Conectar frontend con API
   - Implementar manejo de errores
   - Configurar logging

2. **Testing**
   
   - Pruebas unitarias
   - Pruebas de integración
   - Pruebas de usuario

## ⚙️ Configuración de Desarrollo

### appsettings.json (API)

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourdomain.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:your-server.database.windows.net,1433;..."
  },
  "BlobStorage": {
    "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
    "ContainerName": "project-documents"
  }
}
```

### appsettings.json (Blazor)

```json
{
  "AzureAd": {
    "Authority": "https://login.microsoftonline.com/your-tenant-id",
    "ClientId": "your-client-id",
    "ValidateAuthority": true
  },
  "ApiUrl": "https://localhost:7001"
}
```

## 🔍 Consideraciones Importantes

### Principios de Diseño

1. **Simplicidad**: Mantener el código limpio y mantenible
2. **Funcionalidad**: Priorizar features que aporten valor
3. **Escalabilidad**: Diseñar para crecimiento futuro
4. **Performance**: Optimizar consultas y carga de datos
5. **Seguridad**: Validación en cliente y servidor

### Buenas Prácticas

- Usar async/await consistentemente
- Implementar logging estructurado
- Manejar excepciones apropiadamente
- Documentar código con XML comments
- Seguir convenciones de nomenclatura .NET
- Implementar versionado de API

### Validaciones Críticas

- Control Accounts deben tener CAM asignado
- Work Packages no pueden exceder presupuesto del CA
- Fechas de actividades deben respetar dependencias
- Cambios de alcance requieren aprobación
- EVM solo sobre baselines aprobadas

## 📚 Referencias del Project Knowledge

### Documentos Clave

- **ControlAccounts.md**: Estructura completa de cuentas de control
- Reporte9Columnas.md: Reporte requerido.
- **YNEX Templates**: Patrones de diseño UI/UX

### Módulos Implementados

- `UsersModule.cs`: Gestión de usuarios
- `BudgetEndpoints.cs`: Endpoints de presupuesto
- `CostEndpoints.cs`: Endpoints de costos
- `CostService.cs`: Servicio de costos en Web

### Entidades de Dominio

- `ControlAccount.cs`: Entidad principal de control
- `ControlAccountConfiguration.cs`: Configuración EF Core

## 🚀 Próximos Pasos

1. **Validar el plan propuesto** con el equipo de desarrollo
2. **Priorizar módulos** según necesidades del negocio
3. **Definir sprints** y entregables específicos
4. **Configurar ambiente** de desarrollo
5. **Iniciar desarrollo** siguiendo el plan fase por fase

## 📞 Soporte y Consultas

Para dudas o aclaraciones sobre:

- **Arquitectura**: Revisar patrones DDD y Clean Architecture
- **Control Accounts**: Consultar documento ControlAccounts.md en PK
- **Diseño UI**: Referirse a templates YNEX en PK
- **API Contracts**: Ver DTOs definidos en Core project
- **Claude Code**: Usar este documento como referencia principal

---

**Nota**: Este documento debe ser la fuente única de verdad para el desarrollo del sistema. Cualquier cambio debe ser documentado y versionado apropiadamente.
