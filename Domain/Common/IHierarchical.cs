namespace Domain.Common
{
    /// <summary>
    /// Interfaz para entidades jerárquicas.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad jerárquica.</typeparam>
    public interface IHierarchical<T>
    {
        Guid? ParentId { get; }
        T? Parent { get; }
        ICollection<T> Children { get; }
        int Level { get; }
        string HierarchyPath { get; }
    }
}
