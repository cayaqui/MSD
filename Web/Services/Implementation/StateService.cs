using Web.Services.Interfaces;

namespace Web.Services.Implementation;

public class StateService : IStateService
{
    private readonly Dictionary<string, object> _state = new();
    
    public T? GetState<T>(string key) where T : class
    {
        return _state.TryGetValue(key, out var value) ? value as T : null;
    }
    
    public void SetState<T>(string key, T value) where T : class
    {
        _state[key] = value;
    }
    
    public void RemoveState(string key)
    {
        _state.Remove(key);
    }
    
    public void ClearAll()
    {
        _state.Clear();
    }
}