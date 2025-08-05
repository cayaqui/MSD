using Domain.Common;
using Domain.Entities.WBS;

namespace Domain.Entities.Cost.Core
{
    /// <summary>
    /// Individual line item in a Cost Control Report
    /// Represents detailed breakdown by WBS element or work package
    /// </summary>
    public class CostControlReportItem : BaseEntity
    {
        public Guid CostControlReportId { get; private set; }
        public virtual CostControlReport CostControlReport { get; private set; } = null!;
       
        // Link to WBS Element or Work Package
        public Guid? WBSElementId { get; private set; }
        public virtual WBSElement? WBSElement { get; private set; }
        
        public Guid? WorkPackageId { get; private set; }
        public virtual WorkPackageDetails? WorkPackage { get; private set; }
        
        // Item identification
        public string ItemCode { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int SequenceNumber { get; private set; }
        
        // 9 Column Values
        public decimal BudgetedCost { get; private set; }
        public decimal PhysicalProgressPercentage { get; private set; }
        public decimal EarnedValue { get; private set; }
        public decimal ActualCost { get; private set; }
        public decimal CostVariance { get; private set; }
        public decimal ScheduleVariance { get; private set; }
        public decimal CostPerformanceIndex { get; private set; }
        public decimal EstimateAtCompletion { get; private set; }
        
        // Additional tracking
        public string? ResponsiblePerson { get; private set; }
        public string? CostCategory { get; private set; } // Labor, Material, Equipment, Subcontract, etc.
        public bool IsCritical { get; private set; }
        public string? VarianceExplanation { get; private set; }
        
        private CostControlReportItem() { } // EF Core constructor

        public CostControlReportItem(
            Guid costControlReportId,
            string itemCode,
            string description,
            int sequenceNumber)
        {
            CostControlReportId = costControlReportId;
            ItemCode = itemCode;
            Description = description;
            SequenceNumber = sequenceNumber;
        }

        public void LinkToWBSElement(Guid wbsElementId)
        {
            WBSElementId = wbsElementId;
            WorkPackageId = null; // Can only be linked to one
        }

        public void LinkToWorkPackage(Guid workPackageId)
        {
            WorkPackageId = workPackageId;
            WBSElementId = null; // Can only be linked to one
        }

        public void UpdateMetrics(
            decimal budgetedCost,
            decimal physicalProgress,
            decimal actualCost,
            string? responsiblePerson = null,
            string? costCategory = null)
        {
            BudgetedCost = budgetedCost;
            PhysicalProgressPercentage = physicalProgress;
            ActualCost = actualCost;
            ResponsiblePerson = responsiblePerson;
            CostCategory = costCategory;
            
            CalculateMetrics();
        }

        private void CalculateMetrics()
        {
            // Calculate Earned Value
            EarnedValue = BudgetedCost * (PhysicalProgressPercentage / 100);
            
            // Calculate variances
            CostVariance = EarnedValue - ActualCost;
            ScheduleVariance = EarnedValue - BudgetedCost;
            
            // Calculate performance indices
            CostPerformanceIndex = ActualCost > 0 ? EarnedValue / ActualCost : 0;
            
            // Calculate EAC using CPI method
            if (CostPerformanceIndex > 0 && BudgetedCost > 0)
            {
                EstimateAtCompletion = ActualCost + (BudgetedCost - EarnedValue) / CostPerformanceIndex;
            }
            else
            {
                EstimateAtCompletion = BudgetedCost;
            }
            
            // Mark as critical if significant variance
            IsCritical = Math.Abs(CostVariance) > BudgetedCost * 0.1m || // 10% cost variance
                        Math.Abs(ScheduleVariance) > BudgetedCost * 0.1m; // 10% schedule variance
        }

        public void AddVarianceExplanation(string explanation)
        {
            VarianceExplanation = explanation;
        }
    }
}