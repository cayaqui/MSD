namespace Web.Services.Interfaces;

public interface INavigationService
{
    void NavigateTo(string uri, bool forceLoad = false);
    void NavigateToLogin(string? returnUrl = null);
    void NavigateBack();
}