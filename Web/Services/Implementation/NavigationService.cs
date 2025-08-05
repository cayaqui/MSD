using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class NavigationService : INavigationService
{
    private readonly NavigationManager _navigationManager;
    private readonly IJSRuntime _jsRuntime;
    
    public NavigationService(NavigationManager navigationManager, IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _jsRuntime = jsRuntime;
    }
    
    public void NavigateTo(string uri, bool forceLoad = false)
    {
        _navigationManager.NavigateTo(uri, forceLoad);
    }
    
    public void NavigateToLogin(string? returnUrl = null)
    {
        var url = "authentication/login";
        if (!string.IsNullOrEmpty(returnUrl))
        {
            url += $"?returnUrl={Uri.EscapeDataString(returnUrl)}";
        }
        _navigationManager.NavigateTo(url);
    }
    
    public async void NavigateBack()
    {
        await _jsRuntime.InvokeVoidAsync("history.back");
    }
}