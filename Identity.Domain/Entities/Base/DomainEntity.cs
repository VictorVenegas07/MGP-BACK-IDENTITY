using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Domain.Entities.Base;

public class DomainEntity : ISoftDelete
{
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; private set; }

    [Column("UpdatedAt")]
    public DateTime UpdatedAt { get; private set; }

    [Column("DeletedOn")]
    public DateTime? DeletedOn { get; set; }

    [Column("IsDeleted")]
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Sets the creation timestamp to the current UTC time.
    /// </summary>
    public void MarkAsCreated() => CreatedAt = DateTime.UtcNow;

    /// <summary>
    /// Updates the timestamp for the last modification to the current UTC time.
    /// </summary>
    public void MarkAsUpdated() => UpdatedAt = DateTime.UtcNow;

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    public void MarkAsDeleted() => IsDeleted = true;

    /// <summary>
    /// Sets the deletion timestamp to the current UTC time.
    /// </summary>
    public void SetDelete() => DeletedOn = DateTime.UtcNow;
}

