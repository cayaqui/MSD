using Domain.Common;



namespace Domain.Entities.Cost.Core
{
    /// <summary>
    /// Time-phased budget distribution for Control Accounts
    /// Supports PMI Performance Measurement Baseline (PMB)
    /// </summary>
    public class TimePhasedBudget : BaseEntity
    {
        public Guid ControlAccountId { get; private set; }
        public virtual ControlAccount ControlAccount { get; private set; } = null!;
        public DateTime PeriodStart { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        public string PeriodType { get; private set; } = "Monthly"; // Daily, Weekly, Monthly, Quarterly
        // Budget values
        public decimal PlannedValue { get; private set; } // PV/BCWS for the period
        public decimal CumulativePlannedValue { get; private set; } // Cumulative PV up to this period
        
        // Resource allocation
        public decimal PlannedLaborHours { get; private set; }
        public decimal PlannedLaborCost { get; private set; }
        public decimal PlannedMaterialCost { get; private set; }
        public decimal PlannedEquipmentCost { get; private set; }
        public decimal PlannedSubcontractCost { get; private set; }
        public decimal PlannedOtherCost { get; private set; }
        
        // Total for the period
        public decimal TotalPlannedCost => PlannedLaborCost + PlannedMaterialCost + 
                                           PlannedEquipmentCost + PlannedSubcontractCost + 
                                           PlannedOtherCost;
        
        // Status
        public bool IsBaseline { get; private set; }
        public DateTime? BaselineDate { get; private set; }
        public int? RevisionNumber { get; private set; }
        
        private TimePhasedBudget() { } // EF Core constructor

        public TimePhasedBudget(
            Guid controlAccountId,
            DateTime periodStart,
            DateTime periodEnd,
            string periodType,
            decimal plannedValue)
        {
            ControlAccountId = controlAccountId;
            PeriodStart = periodStart;
            PeriodEnd = periodEnd;
            PeriodType = periodType;
            PlannedValue = plannedValue;
        }

        public void DistributeResources(
            decimal laborHours,
            decimal laborCost,
            decimal materialCost,
            decimal equipmentCost,
            decimal subcontractCost,
            decimal otherCost)
        {
            PlannedLaborHours = laborHours;
            PlannedLaborCost = laborCost;
            PlannedMaterialCost = materialCost;
            PlannedEquipmentCost = equipmentCost;
            PlannedSubcontractCost = subcontractCost;
            PlannedOtherCost = otherCost;
        }

        public void SetCumulativeValue(decimal cumulativeValue)
        {
            CumulativePlannedValue = cumulativeValue;
        }

        public void SetAsBaseline(int revisionNumber)
        {
            IsBaseline = true;
            BaselineDate = DateTime.UtcNow;
            RevisionNumber = revisionNumber;
        }

        public static List<TimePhasedBudget> CreateMonthlyDistribution(
            Guid controlAccountId,
            DateTime startDate,
            DateTime endDate,
            decimal totalBudget,
            string distributionMethod = "Linear")
        {
            var budgets = new List<TimePhasedBudget>();
            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var finalDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1).AddDays(-1);
            
            int totalMonths = 0;
            var tempDate = currentDate;
            while (tempDate <= finalDate)
            {
                totalMonths++;
                tempDate = tempDate.AddMonths(1);
            }
            
            decimal monthlyBudget = totalBudget / totalMonths;
            decimal cumulative = 0;
            
            while (currentDate <= finalDate)
            {
                var periodEnd = currentDate.AddMonths(1).AddDays(-1);
                if (periodEnd > finalDate)
                    periodEnd = finalDate;
                
                var budget = new TimePhasedBudget(
                    controlAccountId,
                    currentDate,
                    periodEnd,
                    "Monthly",
                    monthlyBudget);
                
                cumulative += monthlyBudget;
                budget.SetCumulativeValue(cumulative);
                
                budgets.Add(budget);
                currentDate = currentDate.AddMonths(1);
            }
            
            return budgets;
        }
    }
}