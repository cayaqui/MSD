using Web.Models;
using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class ToastService : IToastService
{
    public event Action<ToastMessage>? OnShow;
    
    public void ShowSuccess(string message, string title = "Success")
    {
        Show(new ToastMessage { Title = title, Message = message, Type = ToastType.Success });
    }
    
    public void ShowError(string message, string title = "Error")
    {
        Show(new ToastMessage { Title = title, Message = message, Type = ToastType.Error });
    }
    
    public void ShowWarning(string message, string title = "Warning")
    {
        Show(new ToastMessage { Title = title, Message = message, Type = ToastType.Warning });
    }
    
    public void ShowInfo(string message, string title = "Information")
    {
        Show(new ToastMessage { Title = title, Message = message, Type = ToastType.Info });
    }
    
    private void Show(ToastMessage toast)
    {
        OnShow?.Invoke(toast);
    }
}