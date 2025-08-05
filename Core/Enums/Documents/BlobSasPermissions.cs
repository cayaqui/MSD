namespace Core.Enums.Documents
{
    public enum BlobSasPermissions
    {
        Read = 1,
        Write = 2,
        Delete = 4,
        List = 8,
        Add = 16,
        Create = 32,
        Update = 64,
        Process = 128
    }
}
