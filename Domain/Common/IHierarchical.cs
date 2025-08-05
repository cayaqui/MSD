namespace Domain.Common
{
    /// <summary>
    /// Interfaz para entidades jer�rquicas.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad jer�rquica.</typeparam>
    public interface IHierarchical<T>
    {
        Guid? ParentId { get; }
        T? Parent { get; }
        ICollection<T> Children { get; }
        int Level { get; }
        string HierarchyPath { get; }
    }
}
