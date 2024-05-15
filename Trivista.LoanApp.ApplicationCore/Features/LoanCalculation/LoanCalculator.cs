using Carter;
using LanguageExt.Common;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanCalculation;

public class LoanCalculatorController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/calculateLoan", HandlePost)
            .WithName("calculateLoan")
            .RequireAuthorization()
            .WithTags("Loan Calculator");
    }

    private static async Task<IResult> HandlePost(IMediator mediator , [FromBody]LoanCalculatorRequestCommand model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x => x);
    }
}

public sealed record LoanCalculatorResultDto(decimal MonthlyRepaymentAmount, decimal TotalRepaymentAmount, int Tenure,
                                decimal LoanAMount, string Message);

/// <inheritdoc />
public sealed record LoanCalculatorRequestCommand(decimal MonthlySalary, decimal LoanAmount, int LoanTenure, 
    decimal AverageMonthlyRepayment): IRequest<Result<LoanCalculatorResultDto>>;

public sealed class LoanCalculatorRequestCommandHandler: IRequestHandler<LoanCalculatorRequestCommand, Result<LoanCalculatorResultDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public LoanCalculatorRequestCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<LoanCalculatorResultDto>> Handle(LoanCalculatorRequestCommand request, CancellationToken cancellationToken)
    {
        var loan = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);
        if(loan == null)
            return new Result<LoanCalculatorResultDto>(ExceptionManager.Manage("Loan Calculator", "An error occured while calculating loan"));
        
        if(request.LoanAmount > loan.MaximumLoanAmount)
            return new Result<LoanCalculatorResultDto>(ExceptionManager.Manage("Loan Calculator", $"Unable to issue loan above {loan.MaximumLoanAmount}"));
        
        if(request.LoanTenure > loan.MaximumTenure)
            return new Result<LoanCalculatorResultDto>(ExceptionManager.Manage("Loan Calculator", $"Unable to issue loan because tenure can not be more than {loan.MaximumTenure}"));
        
        var customerActualCalculableSalary = Loan.ActualCalculableSalary(request.MonthlySalary, request.AverageMonthlyRepayment);
        var customerRepayableMonthlyIncome = Loan.RepayableMonthlyIncome(customerActualCalculableSalary);
        var monthlyRepaymentAmountInterest = Loan.MonthlyRepaymentAmount_Interest(loan.InterestRate, request.LoanAmount);
        var totalRepaymentAmountInterest = Loan.TotalRepaymentAmount_Interest(loan.InterestRate, request.LoanAmount, request.LoanTenure);
        var monthlyRepaymentAmountPrincipal = Loan.MonthlyRepaymentAmount_Principal(request.LoanAmount, request.LoanTenure);
        var totalRepaymentAmountPrincipal = request.LoanAmount;
        var monthlyRepaymentAmount = Loan.MonthlyRepaymentAmount(monthlyRepaymentAmountInterest, monthlyRepaymentAmountPrincipal);
        var totalRepaymentAmount = Loan.TotalRepaymentAmount(totalRepaymentAmountInterest, totalRepaymentAmountPrincipal);

        if(monthlyRepaymentAmount > customerRepayableMonthlyIncome)
            return new LoanCalculatorResultDto(monthlyRepaymentAmount, totalRepaymentAmount, request.LoanTenure,
            request.LoanAmount, "the calculated monthly repayment amount exceeds your repayable monthly income, this might affect/delay the approval of this loan if requested");
        
        if(monthlyRepaymentAmount <= customerRepayableMonthlyIncome)
            return new LoanCalculatorResultDto(monthlyRepaymentAmount, totalRepaymentAmount, request.LoanTenure,
            request.LoanAmount, "you have high chances of getting this loan approved if requested");
        
        return null;

    }
}