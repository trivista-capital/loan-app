using System.Linq.Expressions;
using LinqKit;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Extensions.Specification;

public static class LoanRequestExtension
{
    public static Expression<Func<LoanRequest, bool>> WhereId(this LoanRequest request, Guid id)
    {
        var predicate = PredicateBuilder.New<LoanRequest>(true);
        predicate = predicate.And(x => x.Id == id);

        return predicate;
    }
}