using Web.Services.Interfaces;

namespace Web.Services.Implementation
{
    public class LoadingService : ILoadingService
    {
        public bool IsLoading { get; private set; }
        public string? Message { get; private set; }
        public event Action? OnChange;

        public void Show(string? message = null)
        {
            IsLoading = true;
            Message = message;
            OnChange?.Invoke();
        }

        public void Hide()
        {
            IsLoading = false;
            Message = null;
            OnChange?.Invoke();
        }

        public IDisposable ShowScoped(string? message = null)
        {
            Show(message);
            return new LoadingScope(this);
        }

        private class LoadingScope : IDisposable
        {
            private readonly LoadingService _service;

            public LoadingScope(LoadingService service)
            {
                _service = service;
            }

            public void Dispose()
            {
                _service.Hide();
            }
        }
    }
}