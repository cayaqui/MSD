-- Datos semilla para la tabla Contractors
-- Principales empresas de ingeniería y construcción en Chile

DECLARE @BaseDate DATETIME2 = GETUTCDATE()

-- Insertar Empresas de Ingeniería
INSERT INTO [setup].[Contractors] 
    ([Id], [Code], [Name], [TaxId], [Address], [City], [State], [Country], 
     [ContactName], [ContactEmail], [ContactPhone], [Rating], [Notes], 
     [IsDeleted], [CreatedAt], [CreatedBy])
VALUES
    -- EMPRESAS DE INGENIERÍA (EPCM)
    (NEWID(), 'AUS-CL', 'Ausenco Chile Ltda.', '76.123.456-7', 
     'Av. Apoquindo 3846, Piso 16', 'Las Condes', 'Santiago', 'Chile',
     'Juan Pérez', 'contacto@ausenco.cl', '+56 2 2345 6789', 4.5, 
     'Empresa global de ingeniería EPCM especializada en minería', 
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'FLU-CL', 'Fluor Chile S.A.', '96.825.430-K',
     'Av. Andrés Bello 2457, Torre Costanera', 'Providencia', 'Santiago', 'Chile',
     'María González', 'chile@fluor.com', '+56 2 2340 5000', 4.8,
     'Líder mundial en ingeniería, procura y construcción (EPC)',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'WOR-CL', 'Worley Chile S.A.', '76.234.567-8',
     'Av. Del Valle 945, Ciudad Empresarial', 'Huechuraba', 'Santiago', 'Chile',
     'Pedro Martínez', 'contacto@worley.com', '+56 2 2756 3000', 4.6,
     'Servicios profesionales de ingeniería para energía y recursos',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'HAT-CL', 'Hatch Chile S.A.', '76.345.678-9',
     'Av. Isidora Goyenechea 2939, Piso 11', 'Las Condes', 'Santiago', 'Chile',
     'Ana Silva', 'chile@hatch.com', '+56 2 2896 0000', 4.7,
     'Consultoría en ingeniería y gestión de proyectos mineros',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'WOO-CL', 'Wood Chile SpA', '76.456.789-0',
     'Av. Vitacura 2939, Piso 10', 'Las Condes', 'Santiago', 'Chile',
     'Carlos Rodríguez', 'chile@woodplc.com', '+56 2 2378 9000', 4.4,
     'Servicios técnicos y de ingeniería para industrias de proceso',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'SNC-CL', 'SNC-Lavalin Chile S.A.', '76.567.890-1',
     'Av. El Bosque Norte 0177, Piso 12', 'Las Condes', 'Santiago', 'Chile',
     'Roberto López', 'chile@snclavalin.com', '+56 2 2941 0000', 4.3,
     'Ingeniería y gestión de proyectos complejos',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'ARC-CL', 'Arcadis Chile S.A.', '76.678.901-2',
     'Av. Kennedy 5454, Piso 6', 'Vitacura', 'Santiago', 'Chile',
     'Claudia Muñoz', 'chile@arcadis.com', '+56 2 2381 6000', 4.5,
     'Consultoría en diseño, ingeniería y gestión',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'PYA-CL', 'Pares & Alvarez Ingenieros Asociados S.A.', '96.566.940-K',
     'Av. Los Conquistadores 1700, Piso 15', 'Providencia', 'Santiago', 'Chile',
     'Alejandro Pares', 'contacto@pya.cl', '+56 2 2750 5500', 4.6,
     'Ingeniería nacional especializada en proyectos industriales',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'JRI-CL', 'JRI Ingeniería S.A.', '76.789.012-3',
     'Av. Providencia 1760, Piso 11', 'Providencia', 'Santiago', 'Chile',
     'José Riesco', 'info@jri.cl', '+56 2 2946 2700', 4.4,
     'Ingeniería multidisciplinaria nacional',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'CAD-CL', 'Cade Ingeniería y Desarrollo S.A.', '76.890.123-4',
     'Av. Santa María 2450', 'Providencia', 'Santiago', 'Chile',
     'Fernando Cade', 'contacto@cade.cl', '+56 2 2820 6600', 4.2,
     'Ingeniería y gestión de proyectos mineros e industriales',
     0, @BaseDate, 'SEED'),

    -- EMPRESAS CONSTRUCTORAS
   
        (NEWID(), 'ICSK-CL', 'Ingeniería y Construcción Sigdo Koppers S.A.', '76.012.345-6',
     'Av. Las Condes 11283', 'Las Condes', 'Santiago', 'Chile',
     'Óscar Contreras', 'contacto@icsk.cl', '+56 2 2405 4500', 4.6,
     'División construcción y montaje del grupo SK',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'SAL-CL', 'Salfacorp S.A.', '91.337.000-7',
     'Av. Vitacura 2736, Piso 10', 'Las Condes', 'Santiago', 'Chile',
     'Rodrigo Donoso', 'contacto@salfacorp.com', '+56 2 2549 2300', 4.6,
     'Ingeniería y construcción de obras civiles y montajes',
     0, @BaseDate, 'SEED'),
      (NEWID(), 'EIZ-CL', 'Echeverría Izquierdo S.A.', '93.815.000-6',
     'Av. Apoquindo 3885, Piso 10', 'Las Condes', 'Santiago', 'Chile',
     'Pablo Echeverría', 'contacto@ei.cl', '+56 2 2631 4500', 4.5,
     'Constructora líder en edificación e infraestructura',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'BES-CL', 'Besalco S.A.', '92.434.000-0',
     'Av. Tajamar 183, Piso 3', 'Las Condes', 'Santiago', 'Chile',
     'Víctor Bezanilla', 'contacto@besalco.cl', '+56 2 2688 4400', 4.4,
     'Construcción de obras civiles e infraestructura',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'CVV-CL', 'Claro Vicuña Valenzuela S.A.', '96.898.220-9',
     'Av. Apoquindo 5550, Piso 11', 'Las Condes', 'Santiago', 'Chile',
     'Antonio Claro', 'contacto@cvv.cl', '+56 2 2571 7000', 4.3,
     'Empresa constructora de obras públicas y privadas',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'MAS-CL', 'Mas Errázuriz S.A.', '76.901.234-5',
     'Av. Presidente Kennedy 4700, Piso 7', 'Vitacura', 'Santiago', 'Chile',
     'Cristián Mas', 'contacto@maserrazu.cl', '+56 2 2430 6100', 4.2,
     'Constructora especializada en proyectos mineros',
     0, @BaseDate, 'SEED'),
    

    
    (NEWID(), 'TEC-CL', 'Tecsa S.A.', '96.771.460-5',
     'Camino Lo Echevers 550', 'Quilicura', 'Santiago', 'Chile',
     'Sergio Contardo', 'contacto@tecsa.cl', '+56 2 2437 7200', 4.1,
     'Obras civiles y montajes electromecánicos',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'DSD-CL', 'DSD Construcciones y Montajes S.A.', '76.123.456-7',
     'Av. Del Valle 937', 'Ciudad Empresarial', 'Santiago', 'Chile',
     'Diego Stegmann', 'contacto@dsd.cl', '+56 2 2392 0100', 4.0,
     'Construcción y montajes industriales',
     0, @BaseDate, 'SEED'),c
    
    (NEWID(), 'COM-CL', 'Construcciones y Montajes COM S.A.', '92.083.000-5',
     'Av. Pedro de Valdivia 291', 'Providencia', 'Santiago', 'Chile',
     'Cristián Ocaña', 'contacto@comsa.cl', '+56 2 2520 8500', 4.3,
     'Montajes industriales y construcción modular',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'BRO-CL', 'Brotec S.A.', '96.646.790-4',
     'Av. Eduardo Frei Montalva 6001', 'Conchalí', 'Santiago', 'Chile',
     'Gustavo Vicuña', 'contacto@brotec.cl', '+56 2 2482 5000', 4.4,
     'Construcción de obras civiles y montajes mineros',
     0, @BaseDate, 'SEED'),
    
    (NEWID(), 'ING-CL', 'Ingevec S.A.', '76.493.280-1',
     'Cerro El Plomo 5420, Piso 15', 'Las Condes', 'Santiago', 'Chile',
     'Enrique Besa', 'contacto@ingevec.cl', '+56 2 2393 9300', 4.2,
     'Ingeniería y construcción de edificación',
     0, @BaseDate, 'SEED')
GO

-- Verificar inserción
SELECT 
    [Code],
    [Name],
    [City],
    [Rating],
    CASE 
        WHEN [Code] LIKE '%-CL' AND [Name] LIKE '%Ingeni%' THEN 'Ingeniería'
        WHEN [Code] LIKE '%-CL' AND ([Name] LIKE '%Construct%' OR [Name] LIKE '%Montaje%') THEN 'Constructora'
        ELSE 'Otro'
    END AS [Tipo],
    [Notes]
FROM [setup].[Contractors]
WHERE [IsDeleted] = 0
ORDER BY [Tipo], [Rating] DESC
GO

-- Contar por tipo
SELECT 
    COUNT(*) AS Total,
    SUM(CASE WHEN [Name] LIKE '%Ingeni%' AND NOT [Name] LIKE '%Construc%' THEN 1 ELSE 0 END) AS [Empresas Ingeniería],
    SUM(CASE WHEN [Name] LIKE '%Construc%' OR [Name] LIKE '%Montaje%' THEN 1 ELSE 0 END) AS [Constructoras]
FROM [setup].[Contractors]
WHERE [IsDeleted] = 0
GO