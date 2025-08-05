using Domain.Common;

namespace Application.Interfaces.Auth
{
    public interface IGraphApiService
    {
        Task<AzureAdUser?> GetUserByEmailAsync(string email);
        Task<AzureAdUser?> GetUserByObjectIdAsync(string objectId);
        Task<byte[]?> GetUserPhotoByIdAsync(string userId);
        Task<string?> GetUserPhotoAsDataUrlAsync(string userId);
        Task<string?> GetUserPhotoAsDataUrlWithMimeTypeAsync(string userId);
        Task<IEnumerable<AzureAdUser>> SearchUsersAsync(string searchTerm);
        Task<bool> UserExistsAsync(string email);
    }
}