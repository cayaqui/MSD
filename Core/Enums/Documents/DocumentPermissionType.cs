namespace Core.Enums.Documents;

public enum DocumentPermissionType
{
    Owner = 0,
    ReadWrite = 1,
    ReadOnly = 2,
    Review = 3,
    Approve = 4,
    Comment = 5,
    Download = 6,
    Print = 7,
    Share = 8,
    Custom = 99
}