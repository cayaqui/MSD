using Web.Models;

namespace Web.Services.Interfaces;

public interface IToastService
{
    event Action<ToastMessage>? OnShow;
    
    void ShowSuccess(string message, string title = "Success");
    void ShowError(string message, string title = "Error");
    void ShowWarning(string message, string title = "Warning");
    void ShowInfo(string message, string title = "Information");
}