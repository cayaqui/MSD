using Web.Models.Responses;

namespace Web.Services.Interfaces
{
    public interface IToastService
    {
        event Action<ToastMessage>? OnShow;

        void ShowSuccess(string message);
        void ShowError(string message);
        void ShowWarning(string message);
        void ShowInfo(string message);

        Task ShowSuccessAsync(string message);
        Task ShowErrorAsync(string message);
        Task ShowWarningAsync(string message);
        Task ShowInfoAsync(string message);
    }
}
