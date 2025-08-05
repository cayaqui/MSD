using Domain.Common;
using Domain.Entities.Cost.Control;
using Domain.Entities.Organization.Core;

namespace Domain.Entities.Cost.Core
{
    /// <summary>
    /// Cost Control Report entity representing the 9-column cost control report
    /// Based on Chilean construction industry standards integrated with PMI EVM
    /// </summary>
    public class CostControlReport : BaseEntity
    {
        public Guid ProjectId { get; private set; }
        public virtual Project Project { get; private set; } = null!;
        
        public Guid ControlAccountId { get; private set; }
        public virtual ControlAccount ControlAccount { get; private set; } = null!;
        
        public DateTime ReportDate { get; private set; }
        public string PeriodType { get; private set; } = "Monthly"; // Daily, Weekly, Monthly, Quarterly
        
        // 9 Columns based on Chilean standard practice
        // Column 1: Activity/Work Package Description (from linked entities)
        
        // Column 2: Budgeted Cost (PV - Planned Value / BCWS)
        public decimal BudgetedCost { get; private set; }
        
        // Column 3: Physical Progress %
        public decimal PhysicalProgressPercentage { get; private set; }
        
        // Column 4: Earned Value (EV - BCWP)
        public decimal EarnedValue { get; private set; }
        
        // Column 5: Actual Cost (AC - ACWP)
        public decimal ActualCost { get; private set; }
        
        // Column 6: Cost Variance (CV = EV - AC)
        public decimal CostVariance { get; private set; }
        
        // Column 7: Schedule Variance (SV = EV - PV)
        public decimal ScheduleVariance { get; private set; }
        
        // Column 8: Cost Performance Index (CPI = EV / AC)
        public decimal CostPerformanceIndex { get; private set; }
        
        // Column 9: Estimate at Completion (EAC)
        public decimal EstimateAtCompletion { get; private set; }
        
        // Additional metrics for Chilean context
        public decimal? ExchangeRateUSD { get; private set; } // USD to CLP exchange rate
        public decimal? UFValue { get; private set; } // Unidad de Fomento value for inflation adjustment
        public decimal? ImportedMaterialsPercentage { get; private set; }
        
        // Variance at Completion (VAC = BAC - EAC)
        public decimal VarianceAtCompletion { get; private set; }
        
        // To Complete Performance Index (TCPI = (BAC - EV) / (BAC - AC))
        public decimal? ToCompletePerformanceIndex { get; private set; }
        
        // Schedule Performance Index (SPI = EV / PV)
        public decimal SchedulePerformanceIndex { get; private set; }
        
        // Estimate to Complete (ETC = EAC - AC)
        public decimal EstimateToComplete { get; private set; }
        
        // Report metadata
        public string? Notes { get; private set; }
        public string? ApprovedBy { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        
        // Navigation properties for detailed breakdown
        public virtual ICollection<CostControlReportItem> Items { get; private set; } = new List<CostControlReportItem>();

        private CostControlReport() { } // EF Core constructor

        public CostControlReport(
            Guid projectId,
            Guid controlAccountId,
            DateTime reportDate,
            string periodType)
        {
            ProjectId = projectId;
            ControlAccountId = controlAccountId;
            ReportDate = reportDate;
            PeriodType = periodType;
        }

        public void UpdateMetrics(
            decimal budgetedCost,
            decimal physicalProgress,
            decimal actualCost,
            decimal? exchangeRateUSD = null,
            decimal? ufValue = null)
        {
            BudgetedCost = budgetedCost;
            PhysicalProgressPercentage = physicalProgress;
            ActualCost = actualCost;
            ExchangeRateUSD = exchangeRateUSD;
            UFValue = ufValue;
            
            CalculateEVMMetrics();
        }

        private void CalculateEVMMetrics()
        {
            // Calculate Earned Value (EV = Budget × Physical Progress)
            EarnedValue = BudgetedCost * (PhysicalProgressPercentage / 100);
            
            // Calculate variances
            CostVariance = EarnedValue - ActualCost;
            ScheduleVariance = EarnedValue - BudgetedCost;
            
            // Calculate performance indices
            CostPerformanceIndex = ActualCost > 0 ? EarnedValue / ActualCost : 0;
            SchedulePerformanceIndex = BudgetedCost > 0 ? EarnedValue / BudgetedCost : 0;
            
            // Calculate estimates
            if (ControlAccount != null && ControlAccount.BAC > 0)
            {
                // EAC = AC + (BAC - EV) / CPI
                if (CostPerformanceIndex > 0)
                {
                    EstimateAtCompletion = ActualCost + (ControlAccount.BAC - EarnedValue) / CostPerformanceIndex;
                }
                else
                {
                    EstimateAtCompletion = ControlAccount.BAC;
                }
                
                VarianceAtCompletion = ControlAccount.BAC - EstimateAtCompletion;
                EstimateToComplete = EstimateAtCompletion - ActualCost;
                
                // Calculate TCPI
                decimal workRemaining = ControlAccount.BAC - EarnedValue;
                decimal fundingRemaining = ControlAccount.BAC - ActualCost;
                
                if (fundingRemaining > 0)
                {
                    ToCompletePerformanceIndex = workRemaining / fundingRemaining;
                }
            }
        }

        public void AddReportItem(CostControlReportItem item)
        {
            Items.Add(item);
        }

        public void Approve(string approvedBy)
        {
            ApprovedBy = approvedBy;
            ApprovalDate = DateTime.UtcNow;
        }

        public void AddNotes(string notes)
        {
            Notes = notes;
        }
    }
}