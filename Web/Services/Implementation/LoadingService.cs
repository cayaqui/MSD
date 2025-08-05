using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class LoadingService : ILoadingService
{
    public event Action<bool, string>? OnLoadingChanged;
    
    public void Show(string message = "Loading...")
    {
        OnLoadingChanged?.Invoke(true, message);
    }
    
    public void Hide()
    {
        OnLoadingChanged?.Invoke(false, "");
    }
}