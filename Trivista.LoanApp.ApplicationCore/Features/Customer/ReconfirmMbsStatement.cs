using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class ReconfirmMbsStatement: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/reConfirmMbsStatement", ReConfirmMbsStatementHandler)
            .WithName("Re-Confirm Mbs Statement")
            .WithTags("Customer");
    }
    
    private async Task<IResult> ReConfirmMbsStatementHandler(IMediator mediator, [FromBody]ReConfirmMbsStatementQuery model)
    {
        var result = await mediator.Send(new ReConfirmMbsStatementQuery(model.CustomerId));
        return result.ToOk(x => x);
    }
}

public class ReConfirmMbsStatementQueryValidation : AbstractValidator<ReConfirmMbsStatementQuery>
{
    public ReConfirmMbsStatementQueryValidation()
    {
        RuleFor(x => x.CustomerId).NotNull().NotEmpty().WithMessage("CustomerId must be set");
    }
}

public sealed record ReConfirmMbsStatementQuery(Guid CustomerId) : IRequest<Result<bool>>;

public sealed class ReConfirmMbsStatementHandler : IRequestHandler<ReConfirmMbsStatementQuery, Result<bool>>
{
    private readonly IMbsService _mbsService;

    private readonly IServiceProvider _serviceProvider;
    
    private readonly TrivistaDbContext _trivistaDbContext;

    private readonly ILogger<ReConfirmMbsStatementHandler> _logger;

    public ReConfirmMbsStatementHandler(IMbsService mbsService, 
        IServiceProvider serviceProvider, 
        TrivistaDbContext trivistaDbContext, 
        ILogger<ReConfirmMbsStatementHandler> logger)
    {
        _mbsService = mbsService;
        _serviceProvider = serviceProvider;
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
    }
    
    public async Task<Result<bool>> Handle(ReConfirmMbsStatementQuery request, CancellationToken cancellationToken)
    {
        var validator = new ReConfirmMbsStatementQueryValidation();
        var exceptionResult = await TrivistaValidationException<ReConfirmMbsStatementQueryValidation, ReConfirmMbsStatementQuery>
            .ManageException<bool>(validator, request, cancellationToken, false);
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        
        var customer = await _trivistaDbContext.Customer
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        
        if(customer == null)
            return new Result<bool>(ExceptionManager.Manage("ConfirmMbsStatement", "Customer is invalid"));

        if(string.IsNullOrEmpty(customer.MbsBankStatementTicketAndPassword))
            return new Result<bool>(ExceptionManager.Manage("ConfirmMbsStatement", "Unable to confirm. Please re-initiate process from start again"));
            
        var mbsTicketAndPassword = JsonConvert.DeserializeObject<MbsTicketAndPassword>(customer.MbsBankStatementTicketAndPassword);
        
        var indicinaService = _serviceProvider.GetRequiredService<IIndicina>();
                            var dbContext = _serviceProvider.GetRequiredService<TrivistaDbContext>();
                            
                            var customerToUpdate = await dbContext.Customer
                                .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);

                            var jsonStatementResult = await _mbsService.GetStatementJSONObject(
                                new GetStatementJSONObjectRequestDto()
                                {
                                    TicketNo = mbsTicketAndPassword!.Ticket,
                                    Password = mbsTicketAndPassword.Password
                                });
                            
                            if (jsonStatementResult.Status == "00")
                            {
                                customerToUpdate!.SetMbsBankStatement(jsonStatementResult.Result);

                                dbContext.Customer.Update(customerToUpdate);
                                
                                await dbContext.SaveChangesAsync(cancellationToken);
                                
                                _logger.LogInformation("Successful got statement from mbs and saved to db");
                                
                                var response  = await indicinaService.ProcessStatement(new BankStatementRequest()
                                {
                                    Customer = new()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Email = customerToUpdate.Email,
                                        FirstName = customerToUpdate.FirstName,
                                        LastName = customerToUpdate.LastName,
                                        Phone = customerToUpdate.PhoneNumber
                                    },
                                    BankStatement = new()
                                    {
                                        Type = "mbs",
                                        Content = new()
                                        {
                                            Message = "Successful",
                                            Result = jsonStatementResult.Result,
                                            Status = "00"
                                        }
                                    }
                                });

                                if (string.IsNullOrEmpty(response.Data))
                                {
                                    _logger.LogInformation(response.FailedRequestContent.Message);
                                    return false;
                                }
                                
                                customerToUpdate.SetBankStatementAnalysis(JsonConvert.SerializeObject(response.Data));

                                dbContext.Customer.Update(customerToUpdate);
                                
                                await dbContext.SaveChangesAsync(cancellationToken);
                                
                                _logger.LogInformation("Successful analysis from indicina and saved to db");
                            }
                            else
                            {
                                _logger.LogInformation("Response from jsonStatement is: {Message}", jsonStatementResult.Message);
                                return new Result<bool>(ExceptionManager.Manage("Statement", jsonStatementResult.Message));
                            }
                            _logger.LogInformation("Confirmed statement failed with message: {Message}", jsonStatementResult.Message);

        return false;
    }
}