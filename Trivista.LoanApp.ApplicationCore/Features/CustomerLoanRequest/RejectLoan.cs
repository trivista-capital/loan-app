using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class RejectLoanController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/rejectLoan/{id}", HandleLoanRejection)
            .WithName("RejectLoan")
            .WithTags("Loan Request");
    }
    
    private async Task<IResult> HandleLoanRejection(IMediator mediator, [FromQuery]Guid id)
    {
        var result = await mediator.Send(new RejectLoanCommand(id));
        return result.ToOk(x => x);
    }
}

public class RejectLoanCommandValidation : AbstractValidator<RejectLoanCommand>
{
    public RejectLoanCommandValidation()
    {
        //Kyc details validation
        RuleFor(x => x.Id).NotEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"))
            .NotNull().NotEmpty().WithMessage("Customer is invalid");
    }
}
public sealed record RejectLoanCommand(Guid Id): IRequest<Result<bool>>;

public sealed class RejectLoanCommandHandler : IRequestHandler<RejectLoanCommand, Result<bool>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public RejectLoanCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<bool>> Handle(RejectLoanCommand request, CancellationToken cancellationToken)
    {
        var loanRequestFromDb = await _trivistaDbContext.LoanRequest.Include(x=>x.ApprovalWorkflow).FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
        loanRequestFromDb.SetRejectionDateLoan().RejectLoan();
        var isLoanRejected = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        return isLoanRejected > 0 ? true : false;
    }
}