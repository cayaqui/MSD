namespace Web.Services.Interfaces;

public interface ILoadingService
{
    event Action<bool, string>? OnLoadingChanged;
    
    void Show(string message = "Loading...");
    void Hide();
}