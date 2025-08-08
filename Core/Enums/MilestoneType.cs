namespace Core.Enums;

/// <summary>
/// Tipos de hitos del proyecto
/// </summary>
public enum MilestoneType
{
    /// <summary>
    /// Inicio del proyecto
    /// </summary>
    ProjectStart,
    
    /// <summary>
    /// Fin del proyecto
    /// </summary>
    ProjectEnd,
    
    /// <summary>
    /// Puerta de fase - punto de decisión go/no-go
    /// </summary>
    PhaseGate,
    
    /// <summary>
    /// Entregable clave del proyecto
    /// </summary>
    KeyDeliverable,
    
    /// <summary>
    /// Obligación contractual
    /// </summary>
    ContractualObligation,
    
    /// <summary>
    /// Hito de pago
    /// </summary>
    PaymentMilestone,
    
    /// <summary>
    /// Aprobación regulatoria
    /// </summary>
    RegulatoryApproval,
    
    /// <summary>
    /// Aprobación del cliente
    /// </summary>
    ClientApproval,
    
    /// <summary>
    /// Hito contractual
    /// </summary>
    ContractMilestone,
    
    /// <summary>
    /// Otro tipo de hito
    /// </summary>
    Other
}