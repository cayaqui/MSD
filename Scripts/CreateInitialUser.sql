-- Script para crear el usuario inicial en la base de datos
-- Reemplaza los valores con tus datos de Azure AD

DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
DECLARE @EntraId NVARCHAR(128) = 'TU_AZURE_AD_OBJECT_ID'; -- Obtén esto del portal de Azure AD
DECLARE @Email NVARCHAR(256) = 'tu.email@dominio.com';
DECLARE @Name NVARCHAR(256) = 'Tu Nombre';

-- Insertar el usuario
INSERT INTO [dbo].[Users] (
    [Id],
    [EntraId],
    [Email],
    [Name],
    [IsActive],
    [CreatedAt],
    [CreatedBy],
    [UpdatedAt],
    [UpdatedBy]
)
VALUES (
    @UserId,
    @EntraId,
    @Email,
    @Name,
    1, -- IsActive
    GETUTCDATE(),
    'System',
    GETUTCDATE(),
    'System'
);

-- Asignar rol de administrador
INSERT INTO [dbo].[UserRoles] (
    [UserId],
    [Role],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @UserId,
    'System.Admin',
    GETUTCDATE(),
    'System'
);

-- Asignar permisos globales de administrador
INSERT INTO [dbo].[UserPermissions] (
    [Id],
    [UserId],
    [Permission],
    [CreatedAt],
    [CreatedBy]
)
VALUES 
    (NEWID(), @UserId, 'system.admin', GETUTCDATE(), 'System'),
    (NEWID(), @UserId, 'users.manage', GETUTCDATE(), 'System'),
    (NEWID(), @UserId, 'projects.manage', GETUTCDATE(), 'System'),
    (NEWID(), @UserId, 'companies.manage', GETUTCDATE(), 'System');

PRINT 'Usuario administrador creado exitosamente';