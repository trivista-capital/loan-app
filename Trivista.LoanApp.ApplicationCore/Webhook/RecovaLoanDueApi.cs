using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Features.Account;
using Trivista.LoanApp.ApplicationCore.Filters;
using Trivista.LoanApp.ApplicationCore.Services.Payment;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;

namespace Trivista.LoanApp.ApplicationCore.Webhook
{
    public class RecovaLoanDueApi : ICarterModule// Third call. They will pass the loan amount
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/recoverDueLoan/{refId}", RecovaLoanDueQueryHandler)
           .WithName("Recover loan due")
           .WithTags("Recova")
           .RequireCors("AllowSpecificOrigins");
        }

        private async Task<IResult> RecovaLoanDueQueryHandler(
            IMediator mediator,
            IApiKeyValidator apiKeyValidator, 
            HttpContext context,
            [FromRoute]string refId)
        {
            const string ApiKeyHeaderName = "X-API-Key";
            string apiKey = context.Request.Headers[ApiKeyHeaderName]!;

            var isKeyValid = await apiKeyValidator.IsValid(apiKey);
            if (!isKeyValid)
            {
                return Results.Unauthorized();
            }
            var result = await mediator.Send(new RecovaLoanDueQuery(refId));
            return result.ToOk(x => x);
        }
    }

    public sealed record RecovaLoanDueResponse(string LoanReference, decimal amountDue);

    public sealed record RecovaLoanDueQuery(string LoanReference) : IRequest<Result<RecovaLoanDueResponse>>;

    public sealed record RecovaLoanDueQueryHandler : IRequestHandler<RecovaLoanDueQuery, Result<RecovaLoanDueResponse>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;

        private readonly ILogger<RecovaLoanDueQueryHandler> _logger;

        public RecovaLoanDueQueryHandler(TrivistaDbContext trivistaDbContext,
                                               ILogger<RecovaLoanDueQueryHandler> logger)
        {
            _trivistaDbContext = trivistaDbContext;
            _logger = logger;
        }

        public async Task<Result<RecovaLoanDueResponse>> Handle(RecovaLoanDueQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.LoanReference))
                    return new Result<RecovaLoanDueResponse>(ExceptionManager.Manage("Recover loan", "Loanreference can not be null or empty"));

                if (EnumHelpers.IsStringValidGuid(request.LoanReference))
                    return new Result<RecovaLoanDueResponse>(ExceptionManager.Manage("Recover loan", "Loanreference is invalid"));

                var loanId = Guid.Parse(request.LoanReference);

                var loanRequest = await _trivistaDbContext.LoanRequest.AsNoTracking().
                    Include(x => x.RepaymentSchedules).
                           Where(x => x.Id == loanId).
                           Select(x => x).FirstOrDefaultAsync();

                var todayUnpaidAmount = loanRequest!.RepaymentSchedules.Where(x => x.Status == Commons.Enums.ScheduleStatus.Unpaid)
                    .Sum(x => x.RepaymentAmount);

                return new RecovaLoanDueResponse(loanId.ToString(), todayUnpaidAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while recova called RecovaLoanDueQueryHandler");
                return new Result<RecovaLoanDueResponse>(ExceptionManager.Manage("Recover loan", "An error occured"));
            }
        }
    }
}
