namespace Web.Services.Interfaces
{
    /// <summary>
    /// Interface para el componente EzAccordion
    /// </summary>
    public interface IEzAccordion
    {
        /// <summary>
        /// Registra un item en el accordion
        /// </summary>
        void RegisterItem(IEzAccordionItem item);

        /// <summary>
        /// Desregistra un item del accordion
        /// </summary>
        void UnregisterItem(IEzAccordionItem item);

        /// <summary>
        /// Alterna el estado de un item
        /// </summary>
        Task ToggleItemAsync(string itemId);

        /// <summary>
        /// Indica si se permiten múltiples items abiertos
        /// </summary>
        bool AlwaysOpen { get; }
    }

    /// <summary>
    /// Interface para los items del accordion
    /// </summary>
    public interface IEzAccordionItem
    {
        /// <summary>
        /// ID único del item
        /// </summary>
        string ItemId { get; }

        /// <summary>
        /// Indica si el item está expandido
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// Actualiza el estado expandido del item
        /// </summary>
        Task UpdateExpandedStateAsync(bool expanded);
    }

    /// <summary>
    /// EventArgs para eventos del accordion
    /// </summary>
    public class AccordionItemEventArgs : EventArgs
    {
        public string ItemId { get; set; } = string.Empty;
        public IEzAccordionItem? Item { get; set; }
    }
}