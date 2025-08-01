﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;
using Application.Interfaces.Auth;


namespace Infrastructure.Security.Services;

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
        catch (Exception ex) when (ex is ServiceException || ex is Azure.Identity.AuthenticationFailedException)
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
        catch (Exception ex) when (ex is ServiceException || ex is Azure.Identity.AuthenticationFailedException)
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
        catch (Exception ex) when (ex is ServiceException || ex is Azure.Identity.AuthenticationFailedException)
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
}