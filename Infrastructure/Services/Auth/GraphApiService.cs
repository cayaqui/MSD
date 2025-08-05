using Application.Interfaces.Auth;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;


namespace Infrastructure.Services.Auth;

/// <summary>
/// Service to interact with Microsoft Graph API using Managed Identity
/// </summary>
public class GraphApiService : IGraphApiService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GraphApiService> _logger;
    private readonly GraphServiceClient _graphClient;

    public GraphApiService(IConfiguration configuration, ILogger<GraphApiService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _graphClient = CreateGraphClient();
    }

    private GraphServiceClient CreateGraphClient()
    {
        // Use DefaultAzureCredential which works with:
        // - Managed Identity (in Azure)
        // - Azure CLI (local development)
        // - Visual Studio (local development)
        // - Environment variables
        var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
        {
            // Exclude interactive browser authentication
            ExcludeInteractiveBrowserCredential = true,
            // Set the tenant ID for multi-tenant scenarios
            TenantId = _configuration["AzureAd:TenantId"]
        });

        // Create GraphServiceClient with the credential
        var graphClient = new GraphServiceClient(
            credential,
            new[] { "https://graph.microsoft.com/.default" });

        return graphClient;
    }

    public async Task<AzureAdUser?> GetUserByEmailAsync(string email)
    {
        try
        {
            var users = await _graphClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"mail eq '{email}' or userPrincipalName eq '{email}'";
                    requestConfiguration.QueryParameters.Select = new[]
                    {
                        "id", "displayName", "givenName", "surname", "mail",
                        "userPrincipalName", "jobTitle", "department",
                        "officeLocation", "mobilePhone", "accountEnabled"
                    };
                });

            var user = users?.Value?.FirstOrDefault();
            return user != null ? MapToAzureAdUser(user) : null;
        }
        catch (Exception ex) when (ex is ServiceException || ex is AuthenticationFailedException)
        {
            _logger.LogError(ex, "Error getting user by email {Email} from Graph API", email);
            return null;
        }
    }

    public async Task<AzureAdUser?> GetUserByObjectIdAsync(string objectId)
    {
        try
        {
            var user = await _graphClient.Users[objectId]
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = new[]
                    {
                        "id", "displayName", "givenName", "surname", "mail",
                        "userPrincipalName", "jobTitle", "department",
                        "officeLocation", "mobilePhone", "accountEnabled"
                    };
                });

            return user != null ? MapToAzureAdUser(user) : null;
        }
        catch (Exception ex) when (ex is ServiceException || ex is AuthenticationFailedException)
        {
            _logger.LogError(ex, "Error getting user by Object ID {ObjectId} from Graph API", objectId);
            return null;
        }
    }

    public async Task<IEnumerable<AzureAdUser>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            var users = await _graphClient.Users
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Search = $"\"displayName:{searchTerm}\" OR \"mail:{searchTerm}\"";
                    requestConfiguration.QueryParameters.Select = new[]
                    {
                        "id", "displayName", "givenName", "surname", "mail",
                        "userPrincipalName", "jobTitle", "department",
                        "officeLocation", "mobilePhone", "accountEnabled"
                    };
                    requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");
                    requestConfiguration.QueryParameters.Count = true;
                    requestConfiguration.QueryParameters.Top = 50;
                });

            return users?.Value?.Select(MapToAzureAdUser) ?? Enumerable.Empty<AzureAdUser>();
        }
        catch (Exception ex) when (ex is ServiceException || ex is AuthenticationFailedException)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm} from Graph API", searchTerm);
            return Enumerable.Empty<AzureAdUser>();
        }
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var user = await GetUserByEmailAsync(email);
        return user != null && user.AccountEnabled;
    }

    private AzureAdUser MapToAzureAdUser(Microsoft.Graph.Models.User graphUser)
    {
        return new AzureAdUser
        {
            Id = graphUser.Id ?? string.Empty,
            DisplayName = graphUser.DisplayName ?? string.Empty,
            GivenName = graphUser.GivenName ?? string.Empty,
            Surname = graphUser.Surname ?? string.Empty,
            Mail = graphUser.Mail ?? string.Empty,
            UserPrincipalName = graphUser.UserPrincipalName ?? string.Empty,
            JobTitle = graphUser.JobTitle,
            Department = graphUser.Department,
            OfficeLocation = graphUser.OfficeLocation,
            MobilePhone = graphUser.MobilePhone,
            AccountEnabled = graphUser.AccountEnabled ?? false
        };
    }
    public async Task<byte[]> GetUserPhotoAsync(string userId)
    {
        try
        {
            var photoStream = await _graphClient.Users[userId].Photo.Content.GetAsync();

            if (photoStream != null)
            {
                using (var ms = new MemoryStream())
                {
                    await photoStream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }
        catch (ODataError ex)
        {
            Console.WriteLine($"Error: {ex.Error?.Code} - {ex.Error?.Message}");
            return null;
        }
    }
    public async Task<string?> GetUserPhotoAsDataUrlAsync(string userId)
    {
        try
        {
            var photoBytes = await GetUserPhotoAsync(userId);
            if (photoBytes != null)
            {
                string base64String = Convert.ToBase64String(photoBytes);
                return $"data:image/jpeg;base64,{base64String}";
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando Data URL: {ex.Message}");
            return null;
        }
    }
    public async Task<string?> GetUserPhotoAsDataUrlWithMimeTypeAsync(string userId)
    {
        try
        {
            var photoBytes = await GetUserPhotoAsync(userId);
            if (photoBytes != null)
            {
                string mimeType = DetectImageMimeType(photoBytes);
                string base64String = Convert.ToBase64String(photoBytes);
                return $"data:{mimeType};base64,{base64String}";
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando Data URL: {ex.Message}");
            return null;
        }
    }
    private string DetectImageMimeType(byte[] imageBytes)
    {
        if (imageBytes.Length >= 2)
        {
            // JPEG
            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                return "image/jpeg";

            // PNG
            if (imageBytes.Length >= 8 &&
                imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return "image/png";
        }

        return "image/jpeg"; // Por defecto
    }

    public async Task<byte[]?> GetUserPhotoByIdAsync(string userId)
    {
        try
        {
            // ¡Ya no se usa .Request()!
            var photoStream = await _graphClient.Users[userId].Photo.Content.GetAsync();

            if (photoStream != null)
            {
                using (var ms = new MemoryStream())
                {
                    await photoStream.CopyToAsync(ms);
                    return ms.ToArray();
                }
            }
            return null;
        }
        catch (ODataError ex)
        {
            Console.WriteLine($"Error: {ex.Error?.Code} - {ex.Error?.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general: {ex.Message}");
            return null;
        }
    }
}