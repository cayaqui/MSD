using Domain.Common;
using Domain.Entities.Organization.Core;

namespace Domain.Entities.Cost.Core
{
    /// <summary>
    /// Hierarchical account code structure for Chilean integrated cost control
    /// Format: P-XXX-YY-ZZZ-WW (12 characters segmented)
    /// </summary>
    public class AccountCode : BaseEntity
    {
        // Code components
        public string AccountType { get; private set; } = string.Empty; // P, C, G, E (1 char)
        public string ProjectCode { get; private set; } = string.Empty; // XXX (3 chars)
        public string PhaseCode { get; private set; } = string.Empty; // YY (2 chars)
        public string PackageCode { get; private set; } = string.Empty; // ZZZ (3 chars)
        public string SpecificCode { get; private set; } = string.Empty; // WW (2 chars)
        
        // Full code
        public string FullCode => $"{AccountType}-{ProjectCode}-{PhaseCode}-{PackageCode}-{SpecificCode}";
        
        // Descriptive information
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Category { get; private set; } = string.Empty; // Engineering, Equipment, Materials, etc.
        
        // Chilean accounting mapping
        public string? SIICode { get; private set; } // Servicio de Impuestos Internos code
        public string? SIIDescription { get; private set; }
        public bool RequiresIVA { get; private set; } = true;
        public bool EligibleForAcceleratedDepreciation { get; private set; }
        
        // Cost type classification
        public string CostType { get; private set; } = "Direct"; // Direct, Indirect, Contingency
        public string? CostSubType { get; private set; } // Labor, Material, Equipment, Subcontract
        
        // Navigation properties
        public Guid? ProjectId { get; private set; }
        public virtual Project? Project { get; private set; }
        
        public Guid? PhaseId { get; private set; }
        public virtual Phase? Phase { get; private set; }
        
        public bool IsActive { get; set; } = true;
        
        private AccountCode() { } // EF Core constructor

        public AccountCode(
            string accountType,
            string projectCode,
            string phaseCode,
            string packageCode,
            string specificCode,
            string name,
            string category)
        {
            SetAccountType(accountType);
            SetProjectCode(projectCode);
            SetPhaseCode(phaseCode);
            SetPackageCode(packageCode);
            SetSpecificCode(specificCode);
            Name = name;
            Category = category;
        }

        private void SetAccountType(string accountType)
        {
            if (string.IsNullOrWhiteSpace(accountType) || accountType.Length != 1)
                throw new ArgumentException("Account type must be 1 character");
            
            if (!"PCGE".Contains(accountType.ToUpper()))
                throw new ArgumentException("Account type must be P, C, G, or E");
            
            AccountType = accountType.ToUpper();
        }

        private void SetProjectCode(string projectCode)
        {
            if (string.IsNullOrWhiteSpace(projectCode) || projectCode.Length != 3)
                throw new ArgumentException("Project code must be 3 characters");
            
            ProjectCode = projectCode.ToUpper();
        }

        private void SetPhaseCode(string phaseCode)
        {
            if (string.IsNullOrWhiteSpace(phaseCode) || phaseCode.Length != 2)
                throw new ArgumentException("Phase code must be 2 characters");
            
            PhaseCode = phaseCode;
        }

        private void SetPackageCode(string packageCode)
        {
            if (string.IsNullOrWhiteSpace(packageCode) || packageCode.Length != 3)
                throw new ArgumentException("Package code must be 3 characters");
            
            PackageCode = packageCode;
        }

        private void SetSpecificCode(string specificCode)
        {
            if (string.IsNullOrWhiteSpace(specificCode) || specificCode.Length != 2)
                throw new ArgumentException("Specific code must be 2 characters");
            
            SpecificCode = specificCode;
        }

        public void MapToSII(string siiCode, string siiDescription, bool requiresIVA = true)
        {
            SIICode = siiCode;
            SIIDescription = siiDescription;
            RequiresIVA = requiresIVA;
        }

        public void SetCostClassification(string costType, string? costSubType = null)
        {
            if (!new[] { "Direct", "Indirect", "Contingency" }.Contains(costType))
                throw new ArgumentException("Invalid cost type");
            
            CostType = costType;
            CostSubType = costSubType;
        }

        public void SetDepreciationEligibility(bool eligible)
        {
            EligibleForAcceleratedDepreciation = eligible;
        }

        public void LinkToProject(Guid projectId)
        {
            ProjectId = projectId;
        }

        public void LinkToPhase(Guid phaseId)
        {
            PhaseId = phaseId;
        }

        public static class AccountTypes
        {
            public const string Project = "P";
            public const string Control = "C";
            public const string General = "G";
            public const string EVM = "E";
        }

        public static class PhaseCodes
        {
            public const string FeasibilityStudy = "10";
            public const string BasicEngineering = "20";
            public const string DetailEngineering = "30";
            public const string Procurement = "40";
            public const string Construction = "50";
            public const string Commissioning = "60";
            public const string ProjectManagement = "70";
            public const string ProjectClosure = "80";
        }

        public static class PackageRanges
        {
            public const string EngineeringStart = "100";
            public const string EngineeringEnd = "199";
            public const string EquipmentStart = "200";
            public const string EquipmentEnd = "299";
            public const string MaterialsStart = "300";
            public const string MaterialsEnd = "399";
            public const string CivilWorksStart = "400";
            public const string CivilWorksEnd = "499";
            public const string MechanicalStart = "500";
            public const string MechanicalEnd = "599";
            public const string ElectricalStart = "600";
            public const string ElectricalEnd = "699";
            public const string InstrumentationStart = "700";
            public const string InstrumentationEnd = "799";
            public const string ServicesStart = "800";
            public const string ServicesEnd = "899";
            public const string ContingencyStart = "900";
            public const string ContingencyEnd = "999";
        }
    }
}