namespace Core.Enums.Contracts;

public enum ContractType
{
    LumpSum = 1,
    UnitPrice = 2,
    CostPlus = 3,
    TimeAndMaterial = 4,
    Turnkey = 5,
    DesignBuild = 6,
    EPC = 7,  // Engineering, Procurement, Construction
    EPCM = 8, // Engineering, Procurement, Construction Management
    BOT = 9,  // Build, Operate, Transfer
    Framework = 10,
    Subcontract = 11,
    ConsultingServices = 12,
    MaintenanceService = 13,
    Other = 99
}
