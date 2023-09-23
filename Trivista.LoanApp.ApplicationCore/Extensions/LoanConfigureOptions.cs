using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class LoanConfigureOptions
{
    public static async Task InitializeLoanConfiguration(this IServiceCollection services, 
        Action<Loan>? loanOptions = null)
    {
        services.Configure(loanOptions);
    }
}