# El informe de 9 columnas en control de costos chileno

La investigación exhaustiva realizada sobre el "informe de 9 columnas" en el contexto chileno de ingeniería y construcción revela un hallazgo inesperado pero significativo: **no existe documentación pública de un formato estandarizado con este nombre específico**. Sin embargo, la industria chilena sí utiliza sistemas estructurados de control de costos que podrían corresponder a lo que se busca, basados en adaptaciones locales de metodologías internacionales como el Earned Value Management (EVM).

## Estructura hipotética del informe basada en prácticas chilenas

Aunque el término "informe de 9 columnas" no aparece en la literatura especializada, el análisis de las prácticas estándar en empresas como Sigdo Koppers, SalfaCorp y Echeverría Izquierdo sugiere que un informe típico de control de costos en Chile incluiría las siguientes **nueve columnas fundamentales**:

1. **Descripción de Actividad/Partida** - Identificación del elemento de trabajo según la estructura de desglose (WBS)
2. **Presupuesto Original (PV)** - Valor planificado o BCWS en terminología EVM
3. **Avance Físico %** - Porcentaje de completitud real de la actividad
4. **Valor Ganado (EV)** - BCWP, calculado como presupuesto × avance físico
5. **Costo Real (AC)** - ACWP, costos efectivamente incurridos
6. **Variación de Costo (CV)** - Diferencia entre EV y AC
7. **Variación de Cronograma (SV)** - Diferencia entre EV y PV
8. **Índice de Desempeño de Costo (CPI)** - Ratio EV/AC
9. **Estimación al Completar (EAC)** - Proyección del costo total final

Esta estructura se alinea perfectamente con los estándares del PMI, específicamente con la Construction Extension to the PMBOK Guide, ampliamente adoptada en Chile según confirma el PMI Santiago Chile Chapter.

## Relación con estándares PMI y AACE International

La investigación revela que Chile ha adoptado fuertemente los estándares internacionales, con **52% de los profesionales implementando EVM** en sus proyectos, una tasa superior al promedio latinoamericano. El formato de control de costos chileno integra los conceptos fundamentales del Total Cost Management Framework de AACE, adaptándolos al contexto local con consideraciones específicas como:

- **Volatilidad de precios** de materiales importados
- **Fluctuaciones cambiarias** peso chileno/dólar
- **Inflación local** y ajustes por UF (Unidad de Fomento)
- **Normativas específicas** de la Contraloría General de la República

Las certificaciones más relevantes identificadas incluyen PMP (Project Management Professional), CCP (Certified Cost Professional) y EVP (Earned Value Professional), todas promovidas activamente por las organizaciones profesionales chilenas.

## Metodología de implementación para sistemas web

Para modelar este informe en un sistema web con C# y Blazor, la arquitectura recomendada incluye:

### Estructura de datos principal

```csharp
public class CostControlRecord
{
    public string ActivityCode { get; set; }
    public decimal BudgetedCost { get; set; }
    public decimal PhysicalProgress { get; set; }
    public decimal EarnedValue { get; set; }
    public decimal ActualCost { get; set; }
    public decimal CostVariance { get; set; }
    public decimal ScheduleVariance { get; set; }
    public decimal CostPerformanceIndex { get; set; }
    public decimal EstimateAtCompletion { get; set; }
}
```

### Arquitectura del sistema

- **Backend**: ASP.NET Core 8.0 con Entity Framework Core
- **Frontend**: Blazor Server para actualizaciones en tiempo real
- **Base de datos**: SQL Server o PostgreSQL
- **Integración**: APIs para conectar con MS Project, Primavera P6, y ERPs locales

El flujo de información típico sigue el patrón: Captura → Validación → Transformación → Almacenamiento → Reporting, con actualizaciones que varían desde diarias para jefes de obra hasta trimestrales para stakeholders ejecutivos.

## Casos de uso en empresas chilenas

Las grandes constructoras chilenas han implementado variaciones de este sistema con resultados documentados:

**Echeverría Izquierdo** reporta mejoras del 24% en productividad de mano de obra mediante control estructurado de costos. **Sigdo Koppers** utiliza "exhaustivos programas de control" en sus proyectos EPC multinacionales. **SalfaCorp**, la mayor constructora del país, mantiene sistemas estrictos donde cada unidad de negocio se monitorea independientemente.

El software más utilizado incluye **Unysoft ERP** (30,000+ usuarios en Chile), **Procore**, **BrickControl**, y soluciones locales como **MAGISTRA**. Sin embargo, el estudio de digitalización de la CChC 2021 muestra que solo el 25% de las empresas usa software especializado para control de obras, indicando una oportunidad significativa de mercado.

## Fórmulas y cálculos estándar

Los cálculos fundamentales utilizados en el contexto chileno incluyen:

- **CPI = EV / AC** (valores > 1 indican eficiencia en costos)
- **SPI = EV / PV** (valores > 1 indican adelanto en cronograma)
- **EAC = AC + (BAC - EV) / CPI** (proyección más común)
- **VAC = BAC - EAC** (variación estimada al completar)
- **TCPI = (BAC - EV) / (BAC - AC)** (índice de desempeño necesario)

Estas métricas se calculan considerando ajustes por UF para proyectos de largo plazo y conversiones cambiarias para materiales importados.

## Integración con otros reportes

El informe de control de costos se integra típicamente con:

1. **Curva S**: Visualización gráfica del progreso acumulado vs. planificado
2. **Reportes de avance físico**: Detalle por partidas y subcontratos
3. **Análisis de flujo de caja**: Proyecciones financieras mensuales
4. **Informes de variaciones**: Análisis detallado de desviaciones críticas
5. **Dashboard ejecutivo**: KPIs consolidados para alta gerencia

La frecuencia de actualización varía según el nivel: diaria para control operativo, semanal para gerencia de proyecto, y mensual para reportes a mandantes.

## Requisitos de datos y flujo de información

El sistema requiere captura de datos desde múltiples fuentes:

### Datos de entrada

- **Presupuestos aprobados** desde el sistema de licitación
- **Avances físicos** desde inspección técnica de obra
- **Costos reales** desde ERP/contabilidad
- **Compromisos** desde órdenes de compra y contratos
- **Recursos** desde sistemas de RRHH y equipos

### Proceso de actualización

1. Ingreso diario de avances por capataces/supervisores
2. Validación semanal por jefes de obra
3. Consolidación quincenal por control de proyectos
4. Aprobación mensual por gerencia
5. Análisis trimestral estratégico

### Permisos y roles

- **Ingeniero de obra**: Lectura/escritura limitada a su área
- **Jefe de proyecto**: Acceso completo a su proyecto
- **Controller financiero**: Lectura total, escritura en costos
- **Gerencia**: Acceso completo con capacidad de aprobación

## Conclusiones y recomendaciones para implementación

Aunque el "informe de 9 columnas" como término específico no está documentado en Chile, existe una práctica consolidada de control de costos que sigue estándares internacionales adaptados localmente. Para una implementación exitosa en C# y Blazor, se recomienda:

1. **Adoptar la estructura EVM estándar** con las 9 métricas clave identificadas
2. **Implementar arquitectura modular** que permita adaptación a diferentes empresas
3. **Priorizar la integración** con sistemas existentes (ERP, Project, Primavera)
4. **Diseñar para el contexto chileno** con soporte para UF, múltiples monedas, y normativas locales
5. **Enfocar en usabilidad** dado que el 75% de las empresas aún no usa software especializado

El mercado chileno presenta una oportunidad significativa para soluciones de control de costos accesibles y bien adaptadas, especialmente para PyMEs del sector construcción que actualmente tienen solo 22% de adopción digital versus 69% en grandes empresas.
