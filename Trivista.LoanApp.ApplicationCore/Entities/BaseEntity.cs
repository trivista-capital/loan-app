using Trivista.LoanApp.ApplicationCore.Commons.Constants;
using Trivista.LoanApp.ApplicationCore.Exceptions;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public abstract class BaseEntity<T>
{
    protected BaseEntity() { }
    public BaseEntity(T id)
    {
        if (id.Equals(default))
        {
            throw new TrivistaException("Id is invalid", ErrorMessages.BadRequest);
        }
                
    }
    public T Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Deleted { get; set; }
    public Guid LastModifiedBy { get; set; }
    public Guid DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    private static readonly List<object> _events = new List<object>();

    protected void Apply(object @event)
    {
        When(@event);
        _events.Add(@event);
    }
        
    protected static void AddEvents(object @event)
    {
        _events.Add(@event);
    }
    public IEnumerable<object> GetChanges() => _events.AsEnumerable();
    public void ClearChanges() => _events.Clear();
    protected abstract void When(object @event);
}