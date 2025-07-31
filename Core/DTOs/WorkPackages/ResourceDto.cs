using System;
using System.Collections.Generic;
using Core.Enums.Cost;
using Core.Enums.Progress;
using Core.Enums.Projects;

namespace Core.DTOs.WorkPackages;

/// <summary>
/// DTO for Resource allocation
/// </summary>
public class ResourceDto
{
    public Guid Id { get; set; }
    public string ResourceCode { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public ResourceType Type { get; set; }
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? UnitRate { get; set; }
    public decimal PlannedCost { get; set; }
    public decimal ActualCost { get; set; }
}