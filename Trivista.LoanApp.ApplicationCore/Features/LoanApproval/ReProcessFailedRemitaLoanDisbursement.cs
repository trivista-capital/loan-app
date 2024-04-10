using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class ReProcessFailedRemitaLoanDisbursementController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/disbursement/loanRequest/{loanRequestId}/remita/reProcessFailedRemitaDisbursement", ReProcessFailedRemitaLoanDisbursementHandler)
            .WithTags("Remita")
            .WithName("Reprocess Failed Disbursement");
    }

    private async Task<IResult> ReProcessFailedRemitaLoanDisbursementHandler(IMediator mediator, Guid loanRequestId)
    {
        var result = await mediator.Send(new ReProcessFailedRemitaLoanDisbursementCommand(loanRequestId));

        return result.ToOk(x => x);
    }
}

public record ReProcessFailedRemitaLoanDisbursementCommand(Guid RequestId) : IRequest<Result<(string, bool)>>;

public record ReProcessRemitaLoanDisbursementCommandHandler : IRequestHandler<ReProcessFailedRemitaLoanDisbursementCommand, Result<(string, bool)>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly IRemittaService _remitaService;
    
    public ReProcessRemitaLoanDisbursementCommandHandler(TrivistaDbContext trivistaDbContext, 
        IRemittaService remitaService)
    {
        _trivistaDbContext = trivistaDbContext;
        _remitaService = remitaService;
    }
    
    public async Task<Result<(string, bool)>> Handle(ReProcessFailedRemitaLoanDisbursementCommand request, CancellationToken cancellationToken)
    {
        var failedDisbursement = await _trivistaDbContext.
                            FailedRemitaDisbursement.
                            AsNoTracking().
                            Where(x => x.LoanRequestId == request.RequestId).
                            FirstOrDefaultAsync(cancellationToken);
        
        if(failedDisbursement == null)
            return new Result<(string, bool)>(ExceptionManager.Manage("Remita Disbursement", "Loan request with Id not found"));

        var deserializedObject = JsonConvert.DeserializeObject<LoanDisbursementRequestDto>(failedDisbursement!.Payload);

        if (deserializedObject != null)
        {
            var disbursement = await _remitaService.DisburseLoan(deserializedObject!);   
            
            if (disbursement != null)
            {
                if(disbursement.Message.ToLower() != "Successful".ToLower() && disbursement.Status != "00")
                {
                    return new Result<(string, bool)>(ExceptionManager.Manage("Remita Disbursement", disbursement.Message));
                }
                return (disbursement.Message, true);
            }

            if (disbursement == null)
            {
                //Log to data base here for retry.
                return new Result<(string, bool)>(ExceptionManager.Manage("Remita Disbursement",
                    "Unable to reprocess disbursement"));
            }
        }
        return new Result<(string, bool)>(ExceptionManager.Manage("Remita Disbursement", "Unable to reprocess disbursement. Please try again some other time"));
    }
}

