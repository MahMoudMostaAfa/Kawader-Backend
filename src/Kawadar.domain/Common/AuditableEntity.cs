namespace Kawadar.Domain.Common;

public abstract class AuditableEntity : Entity
{

  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }


  protected AuditableEntity() { }

  protected AuditableEntity(Guid id) : base(id)
  {
  }
}