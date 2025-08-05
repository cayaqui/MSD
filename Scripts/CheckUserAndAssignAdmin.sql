-- Script para verificar usuario y asignar permisos de administrador
DECLARE @Email NVARCHAR(256) = 'alejandro.munoz@koffguerrero.com';
DECLARE @UserId UNIQUEIDENTIFIER;

-- Buscar el usuario por email
SELECT @UserId = Id 
FROM [dbo].[Users] 
WHERE Email = @Email;

IF @UserId IS NULL
BEGIN
    PRINT 'Usuario no encontrado con email: ' + @Email;
    PRINT 'Por favor, ejecuta primero el endpoint /api/setup/initialize-admin';
END
ELSE
BEGIN
    PRINT 'Usuario encontrado:';
    SELECT Id, Email, Name, EntraId, IsActive 
    FROM [dbo].[Users] 
    WHERE Id = @UserId;
    
    -- Verificar permisos existentes
    PRINT '';
    PRINT 'Permisos actuales:';
    SELECT PermissionCode, ProjectId, IsGranted, IsActive 
    FROM [dbo].[UserProjectPermissions] 
    WHERE UserId = @UserId;
    
    -- Verificar asignaciones de proyecto
    PRINT '';
    PRINT 'Asignaciones de proyecto:';
    SELECT p.Name as ProjectName, ptm.Role, ptm.IsActive 
    FROM [dbo].[ProjectTeamMembers] ptm
    INNER JOIN [dbo].[Projects] p ON ptm.ProjectId = p.Id
    WHERE ptm.UserId = @UserId;
    
    -- Asignar permisos de administrador si no los tiene
    IF NOT EXISTS (SELECT 1 FROM [dbo].[UserProjectPermissions] WHERE UserId = @UserId AND PermissionCode = 'system.admin')
    BEGIN
        PRINT '';
        PRINT 'Asignando permisos de administrador...';
        
        DECLARE @Now DATETIME2 = GETUTCDATE();
        
        -- Insertar permisos globales de administrador
        INSERT INTO [dbo].[UserProjectPermissions] 
            ([Id], [UserId], [ProjectId], [PermissionCode], [IsGranted], [GrantedAt], [GrantedBy], [IsActive], [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy])
        VALUES 
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'system.admin', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'users.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'users.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.create', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.edit', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.delete', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'projects.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'companies.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'companies.create', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'companies.edit', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'companies.delete', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'companies.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'configuration.view', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System'),
            (NEWID(), @UserId, '00000000-0000-0000-0000-000000000000', 'configuration.manage', 1, @Now, 'System', 1, @Now, 'System', @Now, 'System');
        
        PRINT 'Permisos de administrador asignados exitosamente!';
    END
    ELSE
    BEGIN
        PRINT '';
        PRINT 'El usuario ya tiene permisos de administrador.';
    END
END