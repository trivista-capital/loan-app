using System.Data.Entity;
using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class GetFailedRemitaLoanDisbursementController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/disbursement/remita/failedRemitaDisbursement", GetFailedRemitaLoanDisbursementHandler)
            .WithTags("Remita")
            .WithName("Failed Disbursement");
    }

    private async Task<IResult> GetFailedRemitaLoanDisbursementHandler(IMediator mediator)
    {
        var result = await mediator.Send(new GetAllFailedRemitaLoanDisbursementQuery());

        return result.ToOk(x => x);
    }
}

public class FailedRemitaLoanDisbursementDto
{
    public long Id { get; set; }
    public Guid LoanRequestId { get; set; }

    public bool IsReProcessed { get; set; }

    public bool IsSuccessful { get; set; }

    public static explicit operator FailedRemitaLoanDisbursementDto(FailedRemitaDisbursement model)
    {
        return new FailedRemitaLoanDisbursementDto
        {
            Id = model.Id,
            LoanRequestId = model.LoanRequestId,
            IsReProcessed = model.IsReProcessed,
            IsSuccessful = model.IsSuccessful
        };  
    }
}

public record GetAllFailedRemitaLoanDisbursementQuery() : IRequest<Result<List<FailedRemitaLoanDisbursementDto>>>;

public record GetAllFailedRemitaLoanDisbursementQueryHandler : IRequestHandler<GetAllFailedRemitaLoanDisbursementQuery, Result<List<FailedRemitaLoanDisbursementDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetAllFailedRemitaLoanDisbursementQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<List<FailedRemitaLoanDisbursementDto>>> Handle(GetAllFailedRemitaLoanDisbursementQuery request, CancellationToken cancellationToken)
    {
        var result = await _trivistaDbContext.FailedRemitaDisbursement.AsNoTracking().ToListAsync(cancellationToken);

        var disbursementDto = result.Select(x => (FailedRemitaLoanDisbursementDto)x).ToList();

        return disbursementDto;
    }
}