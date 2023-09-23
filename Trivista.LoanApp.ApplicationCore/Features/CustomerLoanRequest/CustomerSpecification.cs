using System.Linq.Expressions;
using LinqKit;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class CustomerSpecification
{
    public static Expression<Func<Entities.Customer, bool>> WhereId(Guid id)
    {
        var predicate = PredicateBuilder.New<Entities.Customer>(true);
        predicate = predicate.And(x => x.Id == id);

        return predicate;
    }
}