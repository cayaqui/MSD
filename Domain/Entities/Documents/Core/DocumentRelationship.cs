using Core.Enums.Documents;
using Domain.Common;

namespace Domain.Entities.Documents.Core;

public class DocumentRelationship : BaseEntity
{
    public Guid DocumentId { get; set; }
    public virtual Document Document { get; set; } = null!;
    
    public Guid RelatedDocumentId { get; set; }
    public virtual Document RelatedDocument { get; set; } = null!;
    
    public DocumentRelationshipType RelationshipType { get; set; }
    public string? Description { get; set; }
    public DateTime EstablishedDate { get; set; }
    public Guid EstablishedById { get; set; }
}
