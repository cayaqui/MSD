using Domain.Entities.Progress;

namespace Domain.Entities.Organization.Core
{
    /// <summary>
    /// Project entity representing a project within an operation
    /// Aligned with PMBOK/PMI standards
    /// </summary>
    public class Project : BaseEntity, IAuditable, ISoftDelete, IActivatable
    {
        private Project()
        {
            ProjectTeamMembers = new HashSet<ProjectTeamMember>();
        }

        public Project(
            string code,
            string name,
            Guid operationId,
            DateTime plannedStartDate,
            DateTime plannedEndDate,
            decimal totalBudget,
            string currency = "USD"
        )
            : this()
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            OperationId = operationId;
            PlannedStartDate = plannedStartDate;
            PlannedEndDate = plannedEndDate;
            TotalBudget = totalBudget;
            Currency = currency;

            // Initialize defaults
            Status = ProjectStatus.Planning;
            IsActive = true;
            ProgressPercentage = 0;
            CreatedAt = DateTime.UtcNow;

            // Generate WBS Code
            WBSCode = $"P-{code}";
        }

        // Core Properties
        public string Code { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string WBSCode { get; private set; } = string.Empty;

        // Foreign Keys
        public Guid OperationId { get; private set; }
        public Operation Operation { get; private set; } = null!;

        // Project Charter Information (PMBOK)
        public string? ProjectCharter { get; private set; }
        public string? BusinessCase { get; private set; }
        public string? Objectives { get; private set; }
        public string? Scope { get; private set; }
        public string? Deliverables { get; private set; }
        public string? SuccessCriteria { get; private set; }
        public string? Assumptions { get; private set; }
        public string? Constraints { get; private set; }

        // Status and Timeline
        public ProjectStatus Status { get; private set; }
        public DateTime PlannedStartDate { get; private set; }
        public DateTime PlannedEndDate { get; private set; }
        public DateTime? ActualStartDate { get; private set; }
        public DateTime? ActualEndDate { get; private set; }
        public DateTime? BaselineDate { get; private set; }

        // Financial Information
        public decimal TotalBudget { get; private set; }
        public decimal? ApprovedBudget { get; private set; }
        public decimal? ContingencyBudget { get; private set; }
        public decimal? ActualCost { get; private set; }
        public decimal? CommittedCost { get; private set; }
        public string Currency { get; private set; } = "USD";

        // Project Metadata
        public string? ProjectManagerId { get; private set; }
        public string? ProjectManagerName { get; private set; }
        public string? Location { get; private set; }
        public string? Client { get; private set; }
        public string? ContractNumber { get; private set; }
        public string? PurchaseOrderNumber { get; private set; }
        public string? CostCenter { get; private set; }

        // Progress Tracking
        public decimal ProgressPercentage { get; private set; }
        public decimal? PlannedProgress { get; private set; }
        public decimal? EarnedValue { get; private set; }

        // Control Information
        public int? ChangeOrderCount { get; private set; }
        public DateTime? LastProgressUpdate { get; private set; }
        public string? LastProgressUpdateBy { get; private set; }

        // ISoftDelete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // IActivatable
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<ProjectTeamMember> ProjectTeamMembers { get; private set; }
        public virtual ICollection<ControlAccount> ControlAccounts { get; private set; }
        public virtual ICollection<Budget> Budgets { get; private set; }
        public virtual ICollection<CostControlReport> CostControlReports { get; private set; }
        public virtual ICollection<AccountCode> AccountCodes { get; private set; }
        public virtual ICollection<ExchangeRate> ExchangeRates { get; private set; }
        public virtual ICollection<OBSNode> OBSNodes { get; private set; }
        public virtual ICollection<RAM> RAMAssignments { get; private set; }
        public virtual ICollection<WBSElement> WBSElements { get; private set; }
        public virtual ICollection<PlanningPackage> PlanningPackages { get; private set; }
        public virtual ICollection<Milestone> Milestones { get; private set; }
        public virtual ICollection<Phase> Phases { get; private set; }

        // Business Methods
        public void UpdateBasicInfo(
            string name,
            string? description,
            string? location,
            string? client,
            string? contractNumber,
            string currency
        )
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Location = location;
            Client = client;
            ContractNumber = contractNumber;
            Currency = currency ?? Currency;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProjectCharter(
            string? projectCharter,
            string? businessCase,
            string? objectives,
            string? scope,
            string? deliverables,
            string? successCriteria,
            string? assumptions,
            string? constraints
        )
        {
            ProjectCharter = projectCharter;
            BusinessCase = businessCase;
            Objectives = objectives;
            Scope = scope;
            Deliverables = deliverables;
            SuccessCriteria = successCriteria;
            Assumptions = assumptions;
            Constraints = constraints;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePlannedDates(DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("End date must be after start date");

            PlannedStartDate = startDate;
            PlannedEndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateBudget(
            decimal totalBudget,
            decimal? approvedBudget = null,
            decimal? contingencyBudget = null
        )
        {
            if (totalBudget < 0)
                throw new ArgumentException("Budget cannot be negative");

            TotalBudget = totalBudget;

            if (approvedBudget.HasValue)
                ApprovedBudget = approvedBudget.Value;

            if (contingencyBudget.HasValue)
                ContingencyBudget = contingencyBudget.Value;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(ProjectStatus newStatus)
        {
            // Validate status transitions
            if (!IsValidStatusTransition(Status, newStatus))
                throw new InvalidOperationException(
                    $"Cannot transition from {Status} to {newStatus}"
                );

            var previousStatus = Status;
            Status = newStatus;

            // Update actual dates based on status
            if (newStatus == ProjectStatus.Active && !ActualStartDate.HasValue)
            {
                ActualStartDate = DateTime.UtcNow;
            }
            else if (
                (newStatus == ProjectStatus.Completed || newStatus == ProjectStatus.Cancelled)
                && !ActualEndDate.HasValue
            )
            {
                ActualEndDate = DateTime.UtcNow;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignProjectManager(string projectManagerId, string projectManagerName)
        {
            ProjectManagerId =
                projectManagerId ?? throw new ArgumentNullException(nameof(projectManagerId));
            ProjectManagerName =
                projectManagerName ?? throw new ArgumentNullException(nameof(projectManagerName));
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateProgress(decimal progressPercentage, decimal? earnedValue = null)
        {
            if (progressPercentage < 0 || progressPercentage > 100)
                throw new ArgumentException("Progress percentage must be between 0 and 100");

            ProgressPercentage = progressPercentage;

            if (earnedValue.HasValue)
                EarnedValue = earnedValue.Value;

            LastProgressUpdate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCosts(decimal? actualCost, decimal? committedCost)
        {
            if (actualCost.HasValue)
            {
                if (actualCost.Value < 0)
                    throw new ArgumentException("Actual cost cannot be negative");
                ActualCost = actualCost.Value;
            }

            if (committedCost.HasValue)
            {
                if (committedCost.Value < 0)
                    throw new ArgumentException("Committed cost cannot be negative");
                CommittedCost = committedCost.Value;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void SetBaseline(DateTime baselineDate)
        {
            if (Status != ProjectStatus.Planning && Status != ProjectStatus.Active)
                throw new InvalidOperationException(
                    "Can only baseline projects in Planning or Active status"
                );

            BaselineDate = baselineDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Validation Methods
        private bool IsValidStatusTransition(ProjectStatus from, ProjectStatus to)
        {
            return (from, to) switch
            {
                (ProjectStatus.Planning, ProjectStatus.Active) => true,
                (ProjectStatus.Planning, ProjectStatus.Cancelled) => true,
                (ProjectStatus.Active, ProjectStatus.OnHold) => true,
                (ProjectStatus.Active, ProjectStatus.Completed) => true,
                (ProjectStatus.Active, ProjectStatus.Cancelled) => true,
                (ProjectStatus.Active, ProjectStatus.Delayed) => true,
                (ProjectStatus.OnHold, ProjectStatus.Active) => true,
                (ProjectStatus.OnHold, ProjectStatus.Cancelled) => true,
                (ProjectStatus.Delayed, ProjectStatus.Active) => true,
                (ProjectStatus.Delayed, ProjectStatus.Cancelled) => true,
                (ProjectStatus.Completed, ProjectStatus.Closed) => true,
                (ProjectStatus.Cancelled, ProjectStatus.Closed) => true,
                _ => false,
            };
        }

        // Calculated Properties
        public decimal GetBudgetVariance()
        {
            if (!ActualCost.HasValue)
                return 0;
            return TotalBudget - ActualCost.Value;
        }

        public decimal GetBudgetVariancePercentage()
        {
            if (!ActualCost.HasValue || TotalBudget == 0)
                return 0;
            return (TotalBudget - ActualCost.Value) / TotalBudget * 100;
        }

        public decimal GetScheduleVariance()
        {
            if (!PlannedProgress.HasValue)
                return 0;
            return ProgressPercentage - PlannedProgress.Value;
        }

        public int GetDuration()
        {
            return (PlannedEndDate - PlannedStartDate).Days;
        }

        public int? GetActualDuration()
        {
            if (!ActualStartDate.HasValue)
                return null;
            var endDate = ActualEndDate ?? DateTime.UtcNow;
            return (endDate - ActualStartDate.Value).Days;
        }

        public bool IsOverBudget()
        {
            return ActualCost.HasValue && ActualCost.Value > TotalBudget;
        }

        public bool IsDelayed()
        {
            return Status == ProjectStatus.Active && DateTime.UtcNow > PlannedEndDate;
        }
    }
}
