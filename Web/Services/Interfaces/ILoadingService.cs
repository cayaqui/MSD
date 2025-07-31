namespace Web.Services.Interfaces
{
    public interface ILoadingService
    {
        bool IsLoading { get; }
        string? Message { get; }
        event Action? OnChange;

        void Show(string? message = null);
        void Hide();
        IDisposable ShowScoped(string? message = null);
    }
}