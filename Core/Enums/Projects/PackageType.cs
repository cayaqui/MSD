namespace Core.Enums.Projects;

public enum PackageType
{

    Engineering = 1,
    Procurement = 2,
    Construction = 3,
    Installation = 4,
    Testing = 5,
    Commissioning = 6,
    Handover = 7,
    EPC = 8,
    EPCM = 9,
    Other = 99
}

public static class PackageTypeExtensions
{
    public static string GetDisplayName(this PackageType type)
    {
        return type switch
        {
            PackageType.Engineering => "Ingeniería",
            PackageType.Procurement => "Procura",
            PackageType.Construction => "Construcción",
            PackageType.Installation => "Instalación",
            PackageType.Testing => "Pruebas",
            PackageType.Commissioning => "Puesta en Marcha",
            PackageType.Handover => "Entrega",
            PackageType.EPC => "EPC",
            PackageType.EPCM => "EPCM",
            PackageType.Other => "Otro",
            _ => type.ToString()
        };
    }

    public static string GetCode(this PackageType type)
    {
        return type switch
        {
            PackageType.Engineering => "ENG",
            PackageType.Procurement => "PRO",
            PackageType.Construction => "CON",
            PackageType.Installation => "INS",
            PackageType.Testing => "TST",
            PackageType.Commissioning => "COM",
            PackageType.Handover => "HAN",
            PackageType.EPC => "EPC",
            PackageType.EPCM => "EPCM",
            PackageType.Other => "OTH",
            _ => "UNK"
        };
    }

    public static string GetIcon(this PackageType type)
    {
        return type switch
        {
            PackageType.Engineering => "bi-tools",
            PackageType.Procurement => "bi-cart",
            PackageType.Construction => "bi-building",
            PackageType.Installation => "bi-wrench",
            PackageType.Testing => "bi-clipboard-check",
            PackageType.Commissioning => "bi-power",
            PackageType.Handover => "bi-box-arrow-right",
            PackageType.EPC => "bi-gear-wide-connected",
            PackageType.EPCM => "bi-gear-wide",
            PackageType.Other => "bi-three-dots",
            _ => "bi-question"
        };
    }

    public static int GetTypicalDuration(this PackageType type)
    {
        // Default duration in days
        return type switch
        {
            PackageType.Engineering => 90,
            PackageType.Procurement => 60,
            PackageType.Construction => 180,
            PackageType.Installation => 30,
            PackageType.Testing => 15,
            PackageType.Commissioning => 30,
            PackageType.Handover => 7,
            PackageType.EPC => 365,
            PackageType.EPCM => 600,
            PackageType.Other => 30,
            _ => 30
        };
    }
}