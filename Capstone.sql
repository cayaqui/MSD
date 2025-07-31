-- =====================================================
-- Script: Datos Iniciales - Capstone Copper
-- Base de datos: sd-d-db-cl
-- Fecha: 2025-01-19
-- Descripción: Crea datos para Capstone Copper y sus operaciones en Chile
-- =====================================================

USE [sd-d-db-cl];
GO

-- =====================================================
-- PASO 1: Crear la Compañía (Capstone Copper)
-- =====================================================
PRINT 'Creando compañía Capstone Copper...';

DECLARE @CompanyId UNIQUEIDENTIFIER = NEWID();
DECLARE @Today DATETIME = GETUTCDATE();

INSERT INTO [setup].[Companies] (
    [Id],
    [Code],
    [Name],
    [Description],
    [TaxId],
    [Address],
    [City],
    [State],
    [Country],
    [PostalCode],
    [Phone],
    [Email],
    [Website],
    [DefaultCurrency],
    [FiscalYearStart],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @CompanyId,
    'CAPSTONE',
    'Capstone Copper Corp.',
    'Leading copper producer in the Americas with a portfolio of operating mines and development projects in Chile, USA, and Mexico',
    '13-0534623',  -- Tax ID ficticio para el ejemplo
    '999 West Hastings Street, Suite 1600',
    'Vancouver',
    'British Columbia',
    'Canada',
    'V6C 2W2',
    '+1 604-684-8894',
    'info@capstonecopper.com',
    'https://capstonecopper.com',
        'USD',
    1, -- Enero
    0,
    @Today,
    'SYSTEM'
);

PRINT '✓ Compañía Capstone Copper creada';

-- =====================================================
-- PASO 2: Crear Operaciones en Chile
-- =====================================================
PRINT '';
PRINT 'Creando operaciones en Chile...';

-- Variables para las operaciones
DECLARE @OpSantoDomingoId UNIQUEIDENTIFIER = NEWID();
DECLARE @OpMantoverdeId UNIQUEIDENTIFIER = NEWID();
DECLARE @OpMantosBlanosId UNIQUEIDENTIFIER = NEWID();

-- Operación 1: Santo Domingo (Proyecto en desarrollo)
INSERT INTO [setup].[Operations] (
    [Id],
    [CompanyId],
    [Code],
    [Name],
    [Description],
    [Location],
    [Address],
    [City],
    [State],
    [Country],
    [ManagerName],
    [ManagerEmail],
    [CostCenter],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @OpSantoDomingoId,
    @CompanyId,
    'SD-CHL',
    'Santo Domingo Project',
    'Fully-permitted copper-iron-gold project located 35km northeast of Mantoverde mine. Expected to produce 118kt copper/year with 19-year mine life.',
    'Atacama Region, Chile',
    'Km 9 Ruta C-17',
    'Diego de Almagro',
    'Atacama',
    'Chile',
    'Carlos Mendoza',
    'carlos.mendoza@capstonecopper.com',
    'CC-SD-001',
    0,
    @Today,
    'SYSTEM'
);

-- Operación 2: Mantoverde (70% ownership)
INSERT INTO [setup].[Operations] (
    [Id],
    [CompanyId],
    [Code],
    [Name],
    [Description],
    [Location],
    [Address],
    [City],
    [State],
    [Country],
    [ManagerName],
    [ManagerEmail],
    [CostCenter],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @OpMantoverdeId,
    @CompanyId,
    'MV-CHL',
    'Mantoverde Mine',
    'Open pit oxide heap leach copper mine with recently completed sulphide expansion. 70% ownership by Capstone.',
    'Atacama Region, Chile',
    'Ruta 5 Norte Km 950',
    'Chañaral',
    'Atacama',
    'Chile',
    'Roberto Silva',
    'roberto.silva@capstonecopper.com',
    'CC-MV-001',
    0,
    @Today,
    'SYSTEM'
);

-- Operación 3: Mantos Blancos
INSERT INTO [setup].[Operations] (
    [Id],
    [CompanyId],
    [Code],
    [Name],
    [Description],
    [Location],
    [Address],
    [City],
    [State],
    [Country],
    [ManagerName],
    [ManagerEmail],
    [CostCenter],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @OpMantosBlanosId,
    @CompanyId,
    'MB-CHL',
    'Mantos Blancos Mine',
    'Open-pit copper mine with recent debottlenecking to unlock higher throughput. Phase II brownfield expansion study underway.',
    'Antofagasta Region, Chile',
    'Km 45 Ruta B-170',
    'Baquedano',
    'Antofagasta',
    'Chile',
    'María González',
    'maria.gonzalez@capstonecopper.com',
    'CC-MB-001',
    0,
    @Today,
    'SYSTEM'
);

PRINT '✓ Operaciones creadas: Santo Domingo, Mantoverde, Mantos Blancos';

-- =====================================================
-- PASO 3: Crear Proyecto Santo Domingo
-- =====================================================
PRINT '';
PRINT 'Creando proyecto Santo Domingo...';

DECLARE @ProjectSDId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [setup].[Projects] (
    [Id],
    [OperationId],
    [Code],
    [Name],
    [Description],
    [WBSCode],
    [ProjectCharter],
    [Objectives],
    [Scope],
    [Deliverables],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [TotalBudget],
    [Currency],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @ProjectSDId,
    @OpSantoDomingoId,
    'SD-2024-001',
    'Santo Domingo Copper-Iron Project',
    'Development of the Santo Domingo copper-iron-gold deposits including two open pit mines, concentrator plant, and associated infrastructure.',
    'SD.001',
    'Develop a world-class copper-iron operation in the Atacama region, integrating with Mantoverde to create a mining district producing over 200kt copper/year.',
    '1. Achieve first production by 2028
2. Produce average 118kt copper and 4.2Mt iron ore concentrate annually
3. Maintain first quartile operating costs
4. Implement sustainable mining practices using desalinated water
5. Create value through district integration with Mantoverde',
    'Development and operation of Santo Domingo Sur, Iris, and Iris Norte deposits. Construction of 72ktpd concentrator, desalination pipeline, power infrastructure, and port facilities for concentrate export.',
    '1. Completed detailed engineering and EPC contracts
2. Operational 72ktpd concentrator plant
3. Commissioned desalination water pipeline from Mantoverde
4. Operational port facilities at Punta Totoralillo
5. Trained workforce and operational management systems
6. Environmental compliance certifications',
    '2025-01-01',  -- Planned start
    '2044-12-31',  -- 19 years mine life
    NULL,          -- Not started yet
    NULL,
    1,             -- ProjectStatus.Planning
    2300000000,    -- $2.3 billion
    'USD',
    0,
    @Today,
    'SYSTEM'
);

PRINT '✓ Proyecto Santo Domingo creado';

-- =====================================================
-- PASO 4: Crear Fases del Proyecto Santo Domingo
-- =====================================================
PRINT '';
PRINT 'Creando fases del proyecto Santo Domingo...';

-- Fase 1: Ingeniería y Permisos
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Engineering and Permitting',
    'Detailed engineering, permitting updates, and contractor selection',
    1,
    '2025-01-01',
    '2025-12-31',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

-- Fase 2: Financiamiento
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Project Financing',
    'Secure project financing including debt facilities and strategic partnerships',
    2,
    '2025-06-01',
    '2026-06-30',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

-- Fase 3: Construcción Early Works
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Early Works Construction',
    'Site preparation, access roads, temporary facilities, and pioneer camp',
    3,
    '2026-01-01',
    '2026-12-31',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

-- Fase 4: Construcción Principal
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Main Construction',
    'Construction of concentrator plant, crushing system, conveyors, and main infrastructure',
    4,
    '2026-07-01',
    '2028-06-30',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

-- Fase 5: Comisionamiento y Puesta en Marcha
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Commissioning and Ramp-up',
    'Systems commissioning, operational readiness, and production ramp-up to design capacity',
    5,
    '2028-04-01',
    '2028-12-31',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

-- Fase 6: Operación Comercial
INSERT INTO [setup].[Phases] (
    [Id],
    [ProjectId],
    [Name],
    [Description],
    [Order],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [ProgressPercentage],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    NEWID(),
    @ProjectSDId,
    'Commercial Operations',
    'Full commercial production at design capacity of 72ktpd',
    6,
    '2029-01-01',
    '2044-12-31',
    NULL,
    NULL,
    0, -- NotStarted
    0,
    0,
    @Today,
    'SYSTEM'
);

PRINT '✓ 6 fases del proyecto creadas';

-- =====================================================
-- PASO 5: Crear proyecto Mantoverde-Santo Domingo Integration
-- =====================================================
PRINT '';
PRINT 'Creando proyecto de integración MV-SD...';

DECLARE @ProjectIntegrationId UNIQUEIDENTIFIER = NEWID();

INSERT INTO [setup].[Projects] (
    [Id],
    [OperationId],
    [Code],
    [Name],
    [Description],
    [WBSCode],
    [ProjectCharter],
    [Objectives],
    [Scope],
    [Deliverables],
    [PlannedStartDate],
    [PlannedEndDate],
    [ActualStartDate],
    [ActualEndDate],
    [Status],
    [TotalBudget],
    [Currency],
    [IsDeleted],
    [CreatedAt],
    [CreatedBy]
)
VALUES (
    @ProjectIntegrationId,
    @OpSantoDomingoId,
    'MVSD-INT-001',
    'Mantoverde-Santo Domingo District Integration',
    'Integration plan to create a world-class mining district in Atacama targeting 200kt+ copper/year production',
    'INT.001',
    'Integrate Mantoverde and Santo Domingo operations to unlock synergies and create a world-class copper district with potential for cobalt production.',
    '1. Achieve $80-100M annual operating cost savings
2. Share infrastructure including desalination plant and power
3. Optimize ore processing between both operations
4. Develop cobalt recovery capabilities
5. Establish integrated logistics and port operations',
    'Expansion of Mantoverde desalination plant, shared infrastructure development, integrated mine planning, and cobalt recovery plant feasibility.',
    '1. Expanded desalination capacity to 840 l/s
2. Integrated power distribution network
3. Shared maintenance and technical services
4. Cobalt recovery feasibility study completed
5. Integrated logistics and concentrate handling systems',
    '2024-01-01',
    '2030-12-31',
    '2024-01-01',  -- Already started
    NULL,
    2,             -- ProjectStatus.InProgress
    500000000,     -- $500 million additional
    'USD',
    0,
    @Today,
    'SYSTEM'
);

PRINT '✓ Proyecto de integración MV-SD creado';

-- =====================================================
-- VERIFICACIÓN FINAL
-- =====================================================
PRINT '';
PRINT 'Verificando datos creados...';
PRINT '';

-- Mostrar resumen
SELECT 'Compañías' as [Tipo], COUNT(*) as [Cantidad] FROM [setup].[Companies] WHERE IsDeleted = 0
UNION ALL
SELECT 'Operaciones', COUNT(*) FROM [setup].[Operations] WHERE IsDeleted = 0
UNION ALL
SELECT 'Proyectos', COUNT(*) FROM [setup].[Projects] WHERE IsDeleted = 0
UNION ALL
SELECT 'Fases', COUNT(*) FROM [setup].[Phases] WHERE IsDeleted = 0;

-- Mostrar detalles de los proyectos
PRINT '';
PRINT 'Proyectos creados:';
SELECT 
    p.Code,
    p.Name,
    o.Name as [Operation],
    p.TotalBudget,
    p.Currency,
    CASE p.Status
        WHEN 1 THEN 'Planning'
        WHEN 2 THEN 'InProgress'
        WHEN 3 THEN 'OnHold'
        WHEN 4 THEN 'Completed'
        WHEN 5 THEN 'Cancelled'
        ELSE 'Unknown'
    END as [Status]
FROM [setup].[Projects] p
INNER JOIN [setup].[Operations] o ON p.OperationId = o.Id
WHERE p.IsDeleted = 0;

PRINT '';
PRINT '=====================================================';
PRINT 'Script completado exitosamente';
PRINT '=====================================================';

-- =====================================================
-- SCRIPT DE LIMPIEZA (Por si necesitas revertir)
-- =====================================================
/*
-- Para eliminar todos los datos creados, ejecuta:

DELETE FROM [setup].[Phases] WHERE ProjectId IN (
    SELECT Id FROM [setup].[Projects] WHERE OperationId IN (
        SELECT Id FROM [setup].[Operations] WHERE CompanyId IN (
            SELECT Id FROM [setup].[Companies] WHERE Code = 'CAPSTONE'
        )
    )
);

DELETE FROM [setup].[Projects] WHERE OperationId IN (
    SELECT Id FROM [setup].[Operations] WHERE CompanyId IN (
        SELECT Id FROM [setup].[Companies] WHERE Code = 'CAPSTONE'
    )
);

DELETE FROM [setup].[Operations] WHERE CompanyId IN (
    SELECT Id FROM [setup].[Companies] WHERE Code = 'CAPSTONE'
);

DELETE FROM [setup].[Companies] WHERE Code = 'CAPSTONE';
*/