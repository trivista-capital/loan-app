using Carter;
using ElasticEmail.Model;
using LanguageExt.Common;
using LanguageExt.Pipes;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.Customer;
using Trivista.LoanApp.ApplicationCore.Features.Dto;
using Trivista.LoanApp.ApplicationCore.Features.LoanApproval;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.ExternalClientServiceManagement
{
    public class ClientApiKey: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/admin/apiKeys", ApiKeyCommandHandler)
                .WithName("Generate API Keys")
                .WithTags("Admin");
            //.RequireAuthorization();
        }

        private static async Task<IResult> ApiKeyCommandHandler(IMediator mediator, 
            [FromBody]CustomerApiKeyCommand command)
        {
            var response = await mediator.Send(command);
            return response.ToOk(x => x);
        }
    }

    public sealed record CustomerApiKeyCommand(string email) : IRequest<Result<bool>>;

    public sealed class CustomerApiKeyCommandHandler: IRequestHandler<CustomerApiKeyCommand, Result<bool>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;
        private readonly ILogger<ApproveLoanCommandHandler> _logger;

        public CustomerApiKeyCommandHandler(
            TrivistaDbContext trivistaDbContext,
            ILogger<ApproveLoanCommandHandler> logger)
        {
            this._trivistaDbContext = trivistaDbContext;
            this._logger = logger;
        }

        public async Task<Result<bool>> Handle(CustomerApiKeyCommand request, CancellationToken cancellationToken)
        {
            var apiKey = await _trivistaDbContext.ClientApiKey.FirstOrDefaultAsync(x => x.Email == request.email, cancellationToken);

            if (apiKey != null) 
            {
                return new Result<bool>(ExceptionManager.Manage("Client_Api_Key", "ApiKey already exist"));
            }
            var apiKeyConfig = ClientApiKeyConfiguration.Factory.Build(request.email, Guid.NewGuid().ToString());
            await _trivistaDbContext.ClientApiKey.AddAsync(apiKeyConfig);
            var response = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
            if(response > 0)
            {
                return true;
            }
            return new Result<bool>(ExceptionManager.Manage("Client_Api_Key", "Unable to generate api key"));
        }
    }
}
