using Web.Models.Responses;
using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class ToastService : IToastService
    {
        public event Action<ToastMessage>? OnShow;

        public void ShowSuccess(string message) => Show(message, ToastType.Success);
        public void ShowError(string message) => Show(message, ToastType.Error);
        public void ShowWarning(string message) => Show(message, ToastType.Warning);
        public void ShowInfo(string message) => Show(message, ToastType.Info);

        public Task ShowSuccessAsync(string message)
        {
            ShowSuccess(message);
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string message)
        {
            ShowError(message);
            return Task.CompletedTask;
        }

        public Task ShowWarningAsync(string message)
        {
            ShowWarning(message);
            return Task.CompletedTask;
        }

        public Task ShowInfoAsync(string message)
        {
            ShowInfo(message);
            return Task.CompletedTask;
        }

        private void Show(string message, ToastType type)
        {
            OnShow?.Invoke(new ToastMessage
            {
                Message = message,
                Type = type,
                Timestamp = DateTime.Now
            });
        }
    }
}
