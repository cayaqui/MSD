using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Web.Services;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(
        IAccessTokenProvider provider,
        NavigationManager navigation,
        IConfiguration configuration) 
        : base(provider, navigation)
    {
        var apiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7193/";
        
        ConfigureHandler(
            authorizedUrls: new[] { 
                apiUrl,
                apiUrl.TrimEnd('/'),
                "https://localhost:7193",
                "https://localhost:7193/",
                "https://ez-capst-dev-api-eastus.azurewebsites.net",
                "https://ez-capst-dev-api-eastus.azurewebsites.net/"
            },
            scopes: new[] { "api://25c8d8d0-8323-40fe-8863-e4b22711f572/access_as_user" });
    }
}