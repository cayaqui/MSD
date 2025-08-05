namespace Web.Services.Interfaces;

public interface IStateService
{
    T? GetState<T>(string key) where T : class;
    void SetState<T>(string key, T value) where T : class;
    void RemoveState(string key);
    void ClearAll();
}