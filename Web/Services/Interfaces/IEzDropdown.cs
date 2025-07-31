namespace Web.Services.Interfaces;
/// <summary>
/// Interface para el componente EzDropdown
/// </summary>
public interface IEzDropdown
{
    /// <summary>
    /// Maneja el click en un item del dropdown
    /// </summary>
    /// <param name="shouldClose">Si el dropdown debe cerrarse después del click</param>
    Task OnItemClickAsync(bool shouldClose);

    /// <summary>
    /// Indica si el dropdown cierra automáticamente al hacer click en un item
    /// </summary>
    bool AutoClose { get; }

    /// <summary>
    /// Abre el dropdown
    /// </summary>
    Task OpenAsync();

    /// <summary>
    /// Cierra el dropdown
    /// </summary>
    Task CloseAsync();

    /// <summary>
    /// Alterna el estado del dropdown
    /// </summary>
    Task ToggleAsync();
}
