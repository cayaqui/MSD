# Plan de Cuentas de Control Integrado para Software de Gestión de Proyectos Industriales

## Resumen Ejecutivo

Este plan de cuentas integra los estándares del Project Management Institute (PMI) con los principios contables chilenos, creando una estructura unificada para el control de proyectos de ingeniería y construcción industrial. La arquitectura propuesta permite el cumplimiento simultáneo con el PMBOK Guide, el estándar PMI para Earned Value Management (EVM), y las normativas del Servicio de Impuestos Internos (SII) de Chile bajo IFRS.

## 1. Estructura Jerárquica de Cuentas Basada en PMI-WBS

### 1.1 Arquitectura de Codificación Alfanumérica

El sistema utiliza una estructura de 12 caracteres segmentada para máxima flexibilidad:

```
P-XXX-YY-ZZZ-WW
│  │   │   │   └─ Cuenta Específica (2 dígitos)
│  │   │   └───── Paquete de Trabajo/Control Account (3 caracteres)
│  │   └───────── Fase del Proyecto (2 caracteres)
│  └───────────── Proyecto Individual (3 caracteres)
└──────────────── Tipo de Cuenta (1 carácter)
```

**Tipos de Cuenta (Primer Carácter):**
- **P**: Cuenta de Proyecto (Project Account)
- **C**: Cuenta de Control PMI (Control Account)
- **G**: Cuenta General Contable Chile
- **E**: Cuenta EVM (Earned Value Management)

### 1.2 Niveles Jerárquicos WBS

**Nivel 1 - Proyecto (P-XXX)**
```
P-001: Planta de Procesamiento Minero Norte
P-002: Expansión Terminal Portuario
P-003: Modernización Refinería Central
```

**Nivel 2 - Fases del Proyecto (YY)**
```
10: Estudio de Factibilidad
20: Ingeniería Básica (FEED)
30: Ingeniería de Detalle
40: Procura/Adquisiciones
50: Construcción y Montaje
60: Comisionado y Puesta en Marcha
70: Gestión del Proyecto
80: Cierre del Proyecto
```

**Nivel 3 - Paquetes de Trabajo/Control Accounts (ZZZ)**
```
100-199: Ingeniería
200-299: Equipos Principales
300-399: Materiales a Granel
400-499: Construcción Civil
500-599: Montaje Mecánico
600-699: Instalaciones Eléctricas
700-799: Instrumentación y Control
800-899: Servicios y Soporte
900-999: Contingencias y Reservas
```

## 2. Integración con Plan de Cuentas Chileno

### 2.1 Mapeo con Plan de Cuentas SII para PYMES

**Activos del Proyecto (Código SII: 1.1.40.1 - Productos en Proceso)**
```
G-114-01: Costos Acumulados de Ingeniería
G-114-02: Costos Acumulados de Materiales
G-114-03: Costos Acumulados de Mano de Obra
G-114-04: Costos Indirectos Aplicados
```

**Costos Directos (Código SII: 4.1.10 - Costos Directos por Ventas)**
```
G-411-01: Materiales Directos de Proyecto
G-411-02: Mano de Obra Directa de Proyecto
G-411-03: Servicios de Terceros Directos
G-411-04: Equipamiento Directo
```

### 2.2 Estructura de Control Dual PMI-Chile

Cada transacción del proyecto mantiene doble codificación:
- **Código PMI**: Para control de proyecto y EVM
- **Código SII**: Para cumplimiento tributario y contable

Ejemplo de transacción:
```
Descripción: Compra de acero estructural
Código PMI: P-001-50-512-01 (Proyecto 001, Construcción, Montaje Estructural, Acero)
Código SII: G-114-02 (Productos en Proceso - Materiales)
```

## 3. Categorías Específicas para Proyectos Industriales

### 3.1 Ingeniería y Diseño (Fase 20-30)

**Ingeniería de Procesos (100-119)**
```
P-XXX-20-101-01: Diagramas de Flujo de Proceso (PFD)
P-XXX-20-101-02: Diagramas de Tuberías e Instrumentación (P&ID)
P-XXX-20-101-03: Especificaciones de Equipos Principales
P-XXX-20-101-04: Balance de Masa y Energía
```

**Ingeniería Civil/Estructural (120-139)**
```
P-XXX-30-121-01: Diseño de Fundaciones
P-XXX-30-121-02: Diseño Estructural de Acero
P-XXX-30-121-03: Diseño de Obras Civiles
P-XXX-30-121-04: Caminos y Accesos
```

### 3.2 Procura/Adquisiciones (Fase 40)

**Equipos Principales (200-249)**
```
P-XXX-40-201-01: Equipos Rotativos (Bombas, Compresores)
P-XXX-40-202-01: Equipos Estáticos (Vessels, Intercambiadores)
P-XXX-40-203-01: Equipos Eléctricos Principales
P-XXX-40-204-01: Sistemas de Control DCS/PLC
```

**Materiales a Granel (300-349)**
```
P-XXX-40-301-01: Tubería y Accesorios
P-XXX-40-302-01: Acero Estructural
P-XXX-40-303-01: Cables Eléctricos
P-XXX-40-304-01: Instrumentación de Campo
```

### 3.3 Construcción y Montaje (Fase 50)

**Obras Civiles (400-449)**
```
P-XXX-50-401-01: Movimiento de Tierras
P-XXX-50-402-01: Hormigón Estructural
P-XXX-50-403-01: Pavimentos y Urbanización
```

**Montaje Mecánico (500-549)**
```
P-XXX-50-501-01: Montaje de Equipos Principales
P-XXX-50-502-01: Instalación de Tuberías
P-XXX-50-503-01: Aislamiento y Pintura
```

### 3.4 Comisionado y Puesta en Marcha (Fase 60)

```
P-XXX-60-601-01: Pre-comisionado Mecánico
P-XXX-60-602-01: Comisionado de Sistemas
P-XXX-60-603-01: Pruebas de Performance
P-XXX-60-604-01: Entrenamiento de Operadores
```

## 4. Cuentas de Control según PMI (Control Accounts)

### 4.1 Estructura de Control Account

Cada Control Account sigue el formato:
```
C-XXX-YY-CAM-##
│  │   │   │   └─ Número secuencial
│  │   │   └───── Responsable del Control Account (3 letras)
│  │   └───────── Fase del proyecto
│  └───────────── Proyecto
└──────────────── Identificador Control Account
```

### 4.2 Ejemplos de Control Accounts

```
C-001-30-ENG-01: Control Account Ingeniería de Detalle
C-001-40-PRO-01: Control Account Procura Equipos Principales
C-001-50-CON-01: Control Account Construcción Civil
C-001-50-CON-02: Control Account Montaje Mecánico
```

### 4.3 Componentes del Control Account

Cada Control Account incluye:
- **Work Packages** asociados (nivel más bajo del WBS)
- **Planning Packages** para trabajo futuro
- **Presupuesto autorizado** (BAC - Budget at Completion)
- **Método de medición** de avance
- **Responsable único** (CAM - Control Account Manager)

## 5. Estructura para Earned Value Management (EVM)

### 5.1 Cuentas EVM Principales

```
E-PV-XXX: Planned Value (Valor Planificado/BCWS)
E-EV-XXX: Earned Value (Valor Ganado/BCWP)
E-AC-XXX: Actual Cost (Costo Real/ACWP)
```

### 5.2 Cuentas de Variación y Performance

**Variaciones:**
```
E-CV-XXX: Cost Variance (Variación de Costo) = EV - AC
E-SV-XXX: Schedule Variance (Variación de Cronograma) = EV - PV
```

**Índices de Performance:**
```
E-CPI-XXX: Cost Performance Index = EV / AC
E-SPI-XXX: Schedule Performance Index = EV / PV
```

### 5.3 Cuentas de Pronóstico

```
E-EAC-XXX: Estimate at Completion
E-ETC-XXX: Estimate to Complete
E-VAC-XXX: Variance at Completion = BAC - EAC
```

## 6. Estructura de Costos Directos, Indirectos y Contingencias

### 6.1 Costos Directos

**Mano de Obra Directa (L)**
```
P-XXX-YY-ZZZ-L1: Ingenieros de Campo
P-XXX-YY-ZZZ-L2: Supervisores de Construcción
P-XXX-YY-ZZZ-L3: Trabajadores Especializados
P-XXX-YY-ZZZ-L4: Operadores de Equipos
```

**Materiales Directos (M)**
```
P-XXX-YY-ZZZ-M1: Materiales Permanentes
P-XXX-YY-ZZZ-M2: Materiales Consumibles
P-XXX-YY-ZZZ-M3: Materiales de Instalación
```

**Equipos Directos (E)**
```
P-XXX-YY-ZZZ-E1: Equipos de Proceso
P-XXX-YY-ZZZ-E2: Equipos de Construcción (arriendo)
P-XXX-YY-ZZZ-E3: Herramientas Especiales
```

### 6.2 Costos Indirectos

**Gestión del Proyecto (700-799)**
```
P-XXX-70-701-01: Gerencia de Proyecto
P-XXX-70-702-01: Control de Proyectos
P-XXX-70-703-01: Aseguramiento de Calidad
P-XXX-70-704-01: Seguridad y Medio Ambiente
```

**Instalaciones Temporales (800-849)**
```
P-XXX-70-801-01: Oficinas de Obra
P-XXX-70-802-01: Servicios y Utilidades
P-XXX-70-803-01: Seguridad del Sitio
```

### 6.3 Contingencias y Reservas

**Contingencias Identificadas (900-949)**
```
P-XXX-90-901-01: Contingencia de Diseño (5-10%)
P-XXX-90-902-01: Contingencia de Construcción (10-15%)
P-XXX-90-903-01: Contingencia de Escalación (3-5%)
```

**Reserva de Gestión (950-999)**
```
P-XXX-90-951-01: Reserva de Gestión (5-10%)
P-XXX-90-952-01: Reserva para Cambios de Alcance
```

## 7. Ejemplos Prácticos de Aplicación

### 7.1 Proyecto: Planta de Tratamiento de Aguas Industriales

**Estructura WBS:**
```
P-004: Planta Tratamiento de Aguas
├── P-004-20: Ingeniería Básica
│   ├── C-004-20-ENG-01: Control Account Ingeniería
│   │   ├── P-004-20-101-01: Desarrollo P&IDs
│   │   ├── P-004-20-102-01: Especificaciones Técnicas
│   │   └── P-004-20-103-01: Layout General
│   │
├── P-004-40: Procura
│   ├── C-004-40-PRO-01: Control Account Equipos
│   │   ├── P-004-40-201-01: Bombas Dosificadoras
│   │   ├── P-004-40-201-02: Filtros de Arena
│   │   └── P-004-40-202-01: Tanques de Almacenamiento
│   │
└── P-004-50: Construcción
    ├── C-004-50-CON-01: Control Account Civil
    │   ├── P-004-50-401-01: Fundaciones Equipos
    │   └── P-004-50-402-01: Losas de Hormigón
    │
    └── C-004-50-CON-02: Control Account Mecánico
        ├── P-004-50-501-01: Montaje de Tanques
        └── P-004-50-502-01: Instalación Tuberías
```

### 7.2 Ejemplo de Transacción Integrada

**Compra de Bomba Centrífuga:**
```
Fecha: 15/03/2025
Descripción: Bomba Centrífuga 150 HP

Codificación PMI:
- WBS: P-004-40-201-01
- Control Account: C-004-40-PRO-01
- Tipo de Costo: Directo - Equipo

Codificación Chilena:
- Cuenta SII: 1.1.40.1 (Productos en Proceso)
- Centro de Costo: CC-804 (Proyecto 004)
- Cuenta Auxiliar: 114-02 (Materiales)

Valores EVM:
- PV (marzo 2025): $45,000 USD
- AC (real): $43,500 USD
- EV (50% recepción): $22,500 USD
```

### 7.3 Reporte de Performance Integrado

```
Control Account: C-004-40-PRO-01 (Procura Equipos)
Período: Marzo 2025

Performance EVM:
- BAC: $500,000 USD
- PV:  $200,000 USD
- EV:  $180,000 USD
- AC:  $195,000 USD
- CPI: 0.92 (EV/AC)
- SPI: 0.90 (EV/PV)
- EAC: $543,478 USD

Cumplimiento Chileno:
- IVA Acreditable: $37,050 USD
- Depreciación Acelerada Aplicable: Sí
- Documentación SII: Completa
```

## 8. Mejores Prácticas de Implementación

### 8.1 Configuración Inicial del Software

**1. Parametrización Base:**
- Definir estructura organizacional (OBS)
- Configurar tipos de cambio USD/CLP
- Establecer calendarios de proyecto
- Definir roles y permisos

**2. Carga de Estructura de Cuentas:**
- Importar plan de cuentas mediante plantilla Excel
- Validar mapeo PMI-SII
- Configurar reglas de validación
- Establecer cuentas por defecto

### 8.2 Integración de Sistemas

**Arquitectura Recomendada:**
```
┌─────────────────┐     ┌──────────────┐     ┌─────────────┐
│ Software de     │────▶│ Middleware   │────▶│ ERP         │
│ Proyectos (PMI) │     │ Integración  │     │ Contable    │
└─────────────────┘     └──────────────┘     └─────────────┘
         │                                             │
         └─────────────── Sincronización ─────────────┘
                         Bidireccional
```

**Puntos de Integración Críticos:**
- Maestro de cuentas sincronizado
- Tipos de cambio actualizados
- Transacciones en tiempo real
- Conciliación automática mensual

### 8.3 Control de Cambios

**Proceso de Modificación de Cuentas:**
1. Solicitud formal con justificación
2. Análisis de impacto en reportes
3. Aprobación del PMO
4. Actualización en ambiente de prueba
5. Validación de integridad
6. Implementación en producción
7. Comunicación a usuarios

### 8.4 Capacitación y Adopción

**Plan de Capacitación por Roles:**

**Gerentes de Proyecto:**
- Conceptos EVM y Control Accounts
- Interpretación de reportes
- Gestión de contingencias

**Control de Proyectos:**
- Estructura completa de cuentas
- Técnicas de medición de avance
- Generación de reportes EVM

**Contabilidad:**
- Mapeo PMI-SII
- Conciliación de cuentas
- Cumplimiento tributario

### 8.5 Auditoría y Mejora Continua

**Revisiones Periódicas:**
- Mensual: Validación de transacciones
- Trimestral: Análisis de performance EVM
- Semestral: Revisión estructura de cuentas
- Anual: Auditoría completa de cumplimiento

**Indicadores de Éxito:**
- Precisión de codificación: >98%
- Tiempo de cierre mensual: <5 días
- Variación CPI/SPI: ±10%
- Cumplimiento SII: 100%

## 9. Referencias Normativas

### Referencias PMI:
- **PMBOK Guide 7th Edition** - Project Management Body of Knowledge
- **The Standard for Earned Value Management** - PMI, 2019
- **Practice Standard for Work Breakdown Structures** - Third Edition, PMI
- **Construction Extension to the PMBOK Guide** - PMI, 2016

### Referencias Chilenas:
- **Boletín Técnico N° 85** - Colegio de Contadores de Chile (Adopción IFRS)
- **Manual de Cuentas MiPyme** - Servicio de Impuestos Internos
- **NCG N° 451** - Comisión para el Mercado Financiero
- **Circular N° 47** - SII sobre facturación electrónica

### Estándares Industriales:
- **AACE International Recommended Practice 10S-90** - Cost Engineering Terminology
- **ISO 21500:2012** - Guidance on Project Management
- **CSI MasterFormat** - Construction Specifications Institute

## Conclusión

Este plan de cuentas integrado proporciona una solución robusta que cumple simultáneamente con los estándares internacionales del PMI y las normativas contables chilenas. La estructura modular y escalable permite su adaptación a proyectos industriales de diversa envergadura, manteniendo la trazabilidad necesaria para el control efectivo de costos y el cumplimiento regulatorio. La implementación exitosa requiere un enfoque sistemático, capacitación adecuada del personal y un compromiso organizacional con las mejores prácticas de gestión de proyectos.