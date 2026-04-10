namespace Identity.Domain.Entities.Base;

public class BaseEntity<T> : DomainEntity
{
    public virtual T Id { get; set; } = default!;

}