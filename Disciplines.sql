-- Datos semilla para la tabla Disciplines
-- Eliminar datos existentes si es necesario (opcional)
-- DELETE FROM [setup].[Disciplines] WHERE [CreatedBy] = 'SEED'

-- Insertar disciplinas de ingeniería
INSERT INTO [setup].[Disciplines] 
    ([Id], [Code], [Name], [Description], [Order], [ColorHex], [Icon], [IsEngineering], [IsManagement], [CreatedBy])
VALUES
    -- Disciplinas de Ingeniería
    (NEWID(), 'CIV', 'Civil', 'Ingeniería Civil - Estructuras, concreto, acero', 10, '#8B4513', 'hard-hat', 1, 0, 'SEED'),
    (NEWID(), 'MEC', 'Mecánica', 'Ingeniería Mecánica - Equipos, tuberías, HVAC', 20, '#FF6B35', 'cog', 1, 0, 'SEED'),
    (NEWID(), 'ELE', 'Eléctrica', 'Ingeniería Eléctrica - Alta y baja tensión', 30, '#FFD700', 'zap', 1, 0, 'SEED'),
    (NEWID(), 'INS', 'Instrumentación', 'Instrumentación y Control', 40, '#4169E1', 'activity', 1, 0, 'SEED'),
    (NEWID(), 'TUB', 'Tuberías', 'Ingeniería de Tuberías y Piping', 50, '#20B2AA', 'git-branch', 1, 0, 'SEED'),
    (NEWID(), 'ARQ', 'Arquitectura', 'Diseño Arquitectónico', 60, '#9370DB', 'home', 1, 0, 'SEED'),
    (NEWID(), 'EST', 'Estructural', 'Ingeniería Estructural', 70, '#708090', 'layers', 1, 0, 'SEED'),
    (NEWID(), 'PRO', 'Procesos', 'Ingeniería de Procesos', 80, '#2E8B57', 'git-merge', 1, 0, 'SEED'),
    (NEWID(), 'GEO', 'Geotecnia', 'Estudios Geotécnicos y de Suelos', 90, '#8B7355', 'mountain', 1, 0, 'SEED'),
    (NEWID(), 'HID', 'Hidráulica', 'Ingeniería Hidráulica', 100, '#4682B4', 'droplet', 1, 0, 'SEED'),
    (NEWID(), 'AMB', 'Ambiental', 'Ingeniería Ambiental y Sostenibilidad', 110, '#228B22', 'leaf', 1, 0, 'SEED'),
    (NEWID(), 'SEG', 'Seguridad', 'Seguridad Industrial', 120, '#DC143C', 'shield', 1, 0, 'SEED'),
    
    -- Disciplinas de Gestión/Management
    (NEWID(), 'PMT', 'Project Management', 'Gestión y Dirección de Proyectos', 200, '#4B0082', 'briefcase', 0, 1, 'SEED'),
    (NEWID(), 'PCT', 'Project Control', 'Control de Proyectos - Costos y Cronogramas', 210, '#FF1493', 'trending-up', 0, 1, 'SEED'),
    (NEWID(), 'QAC', 'Control de Calidad', 'Aseguramiento y Control de Calidad', 220, '#FF8C00', 'check-circle', 0, 1, 'SEED'),
    (NEWID(), 'HSE', 'HSE', 'Health, Safety & Environment', 230, '#B22222', 'heart', 0, 1, 'SEED'),
    (NEWID(), 'ADQ', 'Compras', 'Gestión de Compras y Adquisiciones', 240, '#4B0082', 'shopping-cart', 0, 1, 'SEED'),
    (NEWID(), 'CON', 'Construcción', 'Gestión de Construcción', 250, '#D2691E', 'hammer', 0, 1, 'SEED'),
    (NEWID(), 'PLA', 'Planificación', 'Planificación y Programación', 260, '#008080', 'calendar', 0, 1, 'SEED'),
    (NEWID(), 'DOC', 'Documentación', 'Control de Documentos', 270, '#696969', 'file-text', 0, 1, 'SEED'),
    (NEWID(), 'RIS', 'Riesgos', 'Gestión de Riesgos', 280, '#B8860B', 'alert-triangle', 0, 1, 'SEED'),
    (NEWID(), 'COM', 'Puesta en Marcha', 'Commissioning y Startup', 290, '#32CD32', 'play-circle', 0, 1, 'SEED')
GO

-- Verificar inserción
SELECT 
    [Code],
    [Name],
    [ColorHex],
    [Icon],
    CASE 
        WHEN [IsEngineering] = 1 THEN 'Ingeniería'
        WHEN [IsManagement] = 1 THEN 'Gestión'
        ELSE 'Otro'
    END AS [Categoría],
    [Order]
FROM [setup].[Disciplines]
WHERE [IsDeleted] = 0
ORDER BY [Order]
GO
