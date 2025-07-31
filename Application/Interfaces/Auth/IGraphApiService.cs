namespace Application.Interfaces.Auth
{
    public interface IGraphApiService
    {
        Task<AzureAdUser?> GetUserByEmailAsync(string email);
        Task<AzureAdUser?> GetUserByObjectIdAsync(string objectId);
        Task<IEnumerable<AzureAdUser>> SearchUsersAsync(string searchTerm);
        Task<bool> UserExistsAsync(string email);
    }
}