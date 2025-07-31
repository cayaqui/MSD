using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Projects;

public class ProjectListDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string OperationName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public decimal TotalBudget { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? UserRole { get; set; } // Role of current user in this project
}
