using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public abstract class BaseEntity : IComparable<BaseEntity>
{
    public Guid Id { get; set; }

    private List<object> _domainEvents = new List<object>();
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(object eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(object eventItem)
    {
        _domainEvents?.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }

    public Task<IEnumerable<ValidationErrorDetail>> ValidateAsync()
    {
        return Validator.ValidateAsync(this);
    }

    public int CompareTo(BaseEntity? other)
    {
        if (other == null)
        {
            return 1;
        }

        return other!.Id.CompareTo(Id);
    }
}
