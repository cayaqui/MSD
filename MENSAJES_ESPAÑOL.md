# Guía de Mensajes en Español para EzPro

Este documento proporciona ejemplos y guías para mensajes de usuario, errores y validaciones en español para mantener consistencia en toda la aplicación.

## 📋 Principios Generales

1. **Claridad**: Los mensajes deben ser claros y específicos
2. **Consistencia**: Usar el mismo estilo y tono en toda la aplicación
3. **Cortesía**: Mantener un tono profesional y amable
4. **Acción**: Indicar claramente qué debe hacer el usuario

## ❌ Mensajes de Error

### Errores de Validación
```csharp
// Campos requeridos
"El campo {nombre} es requerido"
"Debe ingresar un valor para {campo}"
"Este campo no puede estar vacío"

// Formato inválido
"El formato del {campo} no es válido"
"Ingrese un correo electrónico válido"
"El código debe seguir el formato: XXX-YY-ZZ"

// Rangos y límites
"El valor debe estar entre {min} y {max}"
"La fecha no puede ser anterior a {fecha}"
"El presupuesto no puede exceder {monto}"
"La descripción no puede exceder {n} caracteres"

// Duplicados
"Ya existe un registro con este {campo}"
"El código {valor} ya está en uso"
"No se permiten valores duplicados"
```

### Errores de Proceso
```csharp
// Operaciones CRUD
"Error al crear {entidad}"
"No se pudo actualizar {entidad}"
"Error al eliminar {entidad}"
"No se encontró {entidad} con ID: {id}"

// Permisos
"No tiene permisos para realizar esta acción"
"Acceso denegado al recurso solicitado"
"Requiere rol de {rol} para esta operación"

// Conexión y servidor
"Error de conexión con el servidor"
"El servicio no está disponible temporalmente"
"Tiempo de espera agotado. Intente nuevamente"
"Error inesperado. Contacte al administrador"
```

### Errores de Negocio
```csharp
// WBS
"El elemento padre no puede contener paquetes de trabajo"
"No se puede eliminar un elemento con hijos"
"El código WBS debe ser único dentro del proyecto"

// Presupuesto
"El monto excede el presupuesto disponible"
"No hay fondos suficientes en la cuenta de control"
"El presupuesto total no puede ser negativo"

// Cronograma
"La fecha de fin debe ser posterior a la fecha de inicio"
"Existen dependencias que impiden esta modificación"
"La actividad está en la ruta crítica"

// Control de cambios
"Requiere aprobación antes de proceder"
"El cambio ya fue procesado"
"No se puede modificar un elemento cerrado"
```

## ✅ Mensajes de Éxito

```csharp
// Operaciones básicas
"{Entidad} creado/a exitosamente"
"Cambios guardados correctamente"
"{Entidad} actualizado/a exitosamente"
"{Entidad} eliminado/a exitosamente"

// Operaciones específicas
"Proyecto aprobado exitosamente"
"Presupuesto asignado correctamente"
"Paquete de trabajo convertido exitosamente"
"Archivo importado: {n} registros procesados"
"Exportación completada exitosamente"

// Procesos
"Proceso completado exitosamente"
"Operación finalizada sin errores"
"Sincronización completada"
```

## ⚠️ Mensajes de Advertencia

```csharp
// Confirmaciones
"¿Está seguro que desea eliminar {entidad}?"
"Esta acción no se puede deshacer. ¿Desea continuar?"
"Se perderán los cambios no guardados. ¿Continuar?"

// Alertas
"El presupuesto está cerca del límite ({porcentaje}%)"
"Quedan {n} días para la fecha límite"
"Existen {n} tareas pendientes de aprobación"
"Algunos datos podrían estar desactualizados"

// Impacto
"Esta modificación afectará {n} elementos relacionados"
"Cambiar este valor recalculará los totales"
"La eliminación incluirá todos los elementos hijos"
```

## 💬 Mensajes Informativos

```csharp
// Estados
"No hay datos para mostrar"
"Cargando información..."
"Procesando solicitud..."
"Actualizando datos..."

// Ayuda
"Haga clic para ver más detalles"
"Arrastre para reordenar elementos"
"Use el filtro para refinar resultados"
"Puede exportar estos datos a Excel"

// Resultados
"Se encontraron {n} resultados"
"Mostrando {inicio} a {fin} de {total} registros"
"Página {actual} de {total}"
```

## 🏷️ Etiquetas y Campos

### Campos Comunes
```
Código: "Código"
Nombre: "Nombre"
Descripción: "Descripción"
Estado: "Estado"
Fecha: "Fecha"
Monto: "Monto"
Presupuesto: "Presupuesto"
Responsable: "Responsable"
Proyecto: "Proyecto"
```

### Estados
```
Activo: "Activo"
Inactivo: "Inactivo"
Pendiente: "Pendiente"
Aprobado: "Aprobado"
Rechazado: "Rechazado"
En Proceso: "En Proceso"
Completado: "Completado"
Cancelado: "Cancelado"
```

### Acciones
```
Crear: "Crear"
Editar: "Editar"
Eliminar: "Eliminar"
Guardar: "Guardar"
Cancelar: "Cancelar"
Buscar: "Buscar"
Filtrar: "Filtrar"
Exportar: "Exportar"
Importar: "Importar"
Aprobar: "Aprobar"
Rechazar: "Rechazar"
```

## 📝 Plantillas de Mensajes

### Para Servicios
```csharp
public class MensajesServicio
{
    // Éxito
    public const string CreacionExitosa = "{0} creado/a exitosamente";
    public const string ActualizacionExitosa = "{0} actualizado/a exitosamente";
    public const string EliminacionExitosa = "{0} eliminado/a exitosamente";
    
    // Errores
    public const string ErrorCreacion = "Error al crear {0}: {1}";
    public const string ErrorActualizacion = "Error al actualizar {0}: {1}";
    public const string ErrorEliminacion = "Error al eliminar {0}: {1}";
    public const string NoEncontrado = "No se encontró {0} con ID: {1}";
    
    // Validación
    public const string CampoRequerido = "El campo {0} es requerido";
    public const string FormatoInvalido = "El formato de {0} no es válido";
    public const string ValorDuplicado = "Ya existe un {0} con el valor: {1}";
}
```

### Para Validadores
```csharp
public class MensajesValidacion
{
    // Generales
    public const string Requerido = "es requerido";
    public const string MaximaLongitud = "no puede exceder {MaxLength} caracteres";
    public const string MinimaLongitud = "debe tener al menos {MinLength} caracteres";
    
    // Números
    public const string MayorQueCero = "debe ser mayor que cero";
    public const string RangoValido = "debe estar entre {From} y {To}";
    
    // Fechas
    public const string FechaFutura = "debe ser una fecha futura";
    public const string FechaPasada = "debe ser una fecha pasada";
    public const string FechaInvalida = "no es una fecha válida";
    
    // Específicos
    public const string CodigoWBSFormato = "debe seguir el formato numérico (ej: 1.2.3)";
    public const string EmailInvalido = "debe ser un correo electrónico válido";
}
```

## 🎯 Ejemplos de Uso

### En Validadores FluentValidation
```csharp
RuleFor(x => x.Nombre)
    .NotEmpty().WithMessage("El nombre es requerido")
    .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

RuleFor(x => x.Presupuesto)
    .GreaterThan(0).WithMessage("El presupuesto debe ser mayor que cero")
    .LessThanOrEqualTo(x => x.PresupuestoMaximo)
    .WithMessage("El presupuesto no puede exceder el límite autorizado");
```

### En Servicios
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Error al crear elemento WBS");
    return Result.Failure($"Error al crear el elemento WBS: {ex.Message}");
}
```

### En Páginas Blazor
```razor
Snackbar.Add("Proyecto creado exitosamente", Severity.Success);
Snackbar.Add("Error al cargar los datos", Severity.Error);
Snackbar.Add("Algunos campos requieren atención", Severity.Warning);
```

## 📌 Notas Importantes

1. **Consistencia**: Usar siempre el mismo mensaje para la misma situación
2. **Género**: Concordar el género con la entidad (el proyecto, la tarea)
3. **Formalidad**: Mantener el "usted" implícito, no tutear
4. **Precisión**: Ser específico sobre el error o la acción requerida
5. **Contexto**: Proporcionar suficiente información para que el usuario entienda

## 🔄 Actualizaciones

Este documento debe actualizarse cuando:
- Se agreguen nuevos tipos de mensajes
- Se identifiquen inconsistencias
- Se reciba retroalimentación de usuarios
- Se agreguen nuevas funcionalidades

---

**Última actualización**: @DateTime.Now.ToString("dd/MM/yyyy")