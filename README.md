# EzPro MSD - Sistema de Control de Proyectos

## Descripción
EzPro MSD es un sistema de control de proyectos para ingeniería y construcción, construido con:
- **Backend**: ASP.NET Core 9.0 Web API con minimal APIs (Carter)
- **Frontend**: Blazor WebAssembly con MudBlazor
- **Autenticación**: Azure AD/Entra ID
- **Base de Datos**: Entity Framework Core con SQL Server

## Requisitos Previos
- .NET 9.0 SDK
- Visual Studio 2022 o VS Code
- SQL Server (local o Azure)
- Azure AD tenant configurado

## Configuración Inicial

### 1. Base de Datos
Ejecutar las migraciones desde el directorio Api:
```bash
cd Api
dotnet ef database update
```

### 2. Configuración de Azure AD
Asegurarse de que los siguientes valores estén configurados correctamente:
- En `Api/appsettings.json`: ClientId, TenantId de la aplicación API
- En `Web/wwwroot/appsettings.json`: ClientId de la aplicación SPA

## Ejecutar en Desarrollo

### Opción 1: Usando los scripts
```bash
# Windows PowerShell
./run-dev.ps1

# Windows Command Prompt
run-dev.cmd
```

### Opción 2: Manualmente
1. Abrir terminal en el directorio Api:
```bash
cd Api
dotnet run --launch-profile https
```

2. Abrir otra terminal en el directorio Web:
```bash
cd Web
dotnet run --launch-profile https
```

## URLs de Desarrollo
- **API**: https://localhost:7193
- **Web**: https://localhost:7284
- **Swagger**: https://localhost:7193/swagger

## Solución de Problemas

### Error de CORS
Si aparece el error "A listener indicated an asynchronous response...", verificar:
1. Que la API esté ejecutándose en https://localhost:7193
2. Que el archivo `appsettings.Development.json` exista en ambos proyectos
3. Limpiar caché del navegador y cookies

### Error de Autenticación
1. Verificar que el usuario esté registrado en la base de datos
2. Confirmar que los ClientId en la configuración coincidan con Azure AD
3. Revisar que los scopes estén configurados correctamente

## Estructura del Proyecto
```
MSD/
├── Api/                    # Web API
├── Application/            # Lógica de negocio
├── Core/                   # DTOs y Enums
├── Domain/                 # Entidades del dominio
├── Infrastructure/         # Acceso a datos
├── Web/                    # Blazor WebAssembly
└── CLAUDE.md              # Guía para el asistente de código
```

## Características Principales
- ✅ Autenticación con Azure AD
- ✅ Validación de usuarios contra base de datos
- ✅ Interfaz moderna con MudBlazor
- ✅ Sidebar estilo YNEX
- ✅ Sistema de permisos basado en roles
- ✅ Tema claro/oscuro

## Desarrollo
Ver [CLAUDE.md](CLAUDE.md) para guías de desarrollo y patrones de código.