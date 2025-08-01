﻿namespace Core.DTOs.Companies;

/// <summary>
/// DTO for company with operations summary
/// </summary>
public class CompanyWithOperationsDto : CompanyDto
{
    public List<OperationSummaryDto> Operations { get; set; } = new();
    public int TotalProjects { get; set; }
    public decimal TotalBudget { get; set; }
    public int ActiveProjects { get; set; }
}
