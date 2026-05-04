using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Features.Customer;
using Trivista.LoanApp.ApplicationCore.Filters;
using Trivista.LoanApp.ApplicationCore.Services.Payment;
using Trivista.LoanApp.ApplicationCore.Extensions;
using LanguageExt;

namespace Trivista.LoanApp.ApplicationCore.Webhook
{
    public class RecoverGlToGlNotificationApi : ICarterModule// Second call: endpoint: They will call this endpoint based on the array of schedules we sent when we call them
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/glToGlNotification", RecoverGlToGlNotificationApiCommandHandler)
           .WithName("Gl to gl posting")
           .WithTags("Recova");
        }

        private async Task<IResult> RecoverGlToGlNotificationApiCommandHandler(
            IMediator mediator, 
            IApiKeyValidator apiKeyValidator, HttpContext context, 
            [FromBody]RecoverGlToGlNotificationApiCommand model)
        {
            const string ApiKeyHeaderName = "X-API-Key";
            string apiKey = context.Request.Headers[ApiKeyHeaderName]!;

            var isKeyValid = await apiKeyValidator.IsValid(apiKey);
            if (!isKeyValid)
            {
                return Results.Unauthorized();
            }
            var result = await mediator.Send(model);
            return result.ToOk(x => x);
        }
    }

    public sealed record RecoverGlToGlNotificationApiCommand(string CreditAccountNumber, string DebitAccountNumber,
        decimal Amount, string Narration): IRequest<Result<bool>>;

    public sealed record RecoverGlToGlNotificationApiCommandHandler : IRequestHandler<RecoverGlToGlNotificationApiCommand, Result<bool>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;

        private readonly ILogger<RecoverGlToGlNotificationApiCommandHandler> _logger;

        public RecoverGlToGlNotificationApiCommandHandler(TrivistaDbContext trivistaDbContext,
                                               ILogger<RecoverGlToGlNotificationApiCommandHandler> logger)
        {
            _trivistaDbContext = trivistaDbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(RecoverGlToGlNotificationApiCommand request, CancellationToken cancellationToken)
        {
            //Save detaisl against customer schedule that they have gotten the money

            var loanRequest = await _trivistaDbContext.LoanRequest.Include(x => x.SalaryDetails).
                        Include(x => x.RepaymentSchedules).
                        Where(x => x.SalaryDetails.SalaryAccountNumber == request.DebitAccountNumber).
                        Select(x => x).FirstOrDefaultAsync();

            var schedule = loanRequest!.RepaymentSchedules.FirstOrDefault(x => x.Status == Commons.Enums.ScheduleStatus.Unpaid)!;
            if(schedule == null)
                return new Result<bool>(ExceptionManager.Manage("Recover mandate", $"No unpaid loan for customer with account number: {request.DebitAccountNumber}"));

            schedule!.SetRecover();
            var command = JsonConvert.SerializeObject(request);
            schedule.SetRecoveredDetails(command);
            var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
            if (result > 0)
            {
                return true;
            }
            return new Result<bool>(ExceptionManager.Manage("Gl To Gl Notification", $"Unable to complete posting"));
        }
    }
}
