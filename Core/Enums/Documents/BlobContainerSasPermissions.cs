namespace Core.Enums.Documents
{
    public enum BlobContainerSasPermissions
    {
        Read = 1,
        Write = 2,
        Delete = 4,
        List = 8,
        Add = 16,
        Create = 32
    }
}
