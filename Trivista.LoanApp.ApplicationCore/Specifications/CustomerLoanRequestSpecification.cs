using System.Linq.Expressions;
using LinqKit;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Specifications;

public class CustomerLoanRequestSpecification
{
    public static Expression<Func<LoanRequest, bool>> WhereId(Guid id)
    {
        var predicate = PredicateBuilder.New<LoanRequest>(true);
        predicate = predicate.And(x => x.Id == id);

        return predicate;
    }
    
    public static Expression<Func<LoanRequest, bool>> WhereCustomerId(Guid id)
    {
        var predicate = PredicateBuilder.New<LoanRequest>(true);
        predicate = predicate.And(x => x.CustomerId == id);

        return predicate;
    }
}