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
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.BankCode;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class ConfirmMbsStatementController: ICarterModule 
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/confirmMbsStatement", ConfirmMbsStatementHandler)
            .WithName("Confirm Mbs Statement")
            .WithTags("Customer");
    }
    
   private async Task<IResult> ConfirmMbsStatementHandler(IMediator mediator, [FromBody]ConfirmMbsStatementQuery model)
   {
        var result = await mediator.Send(new ConfirmMbsStatementQuery(model.TicketNumber, model.Password,model.RequestId,  model.CustomerId));
        return result.ToOk(x => x);
   }
}

public class ConfirmMbsStatementQueryValidation : AbstractValidator<ConfirmMbsStatementQuery>
{
    public ConfirmMbsStatementQueryValidation()
    {
        RuleFor(x => x.TicketNumber).NotNull().NotEmpty().WithMessage("Ticket number must be set");
        RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password must be set");
        RuleFor(x => x.CustomerId).NotNull().NotEmpty().WithMessage("CustomerId must be set");
    }
}

public sealed record ConfirmMbsStatementQuery(string TicketNumber, string Password, int RequestId, Guid CustomerId) : IRequest<Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>>;

public sealed class ConfirmMbsStatementHandler : IRequestHandler<ConfirmMbsStatementQuery, Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>>
{
    private readonly IMbsService _mbsService;

    private readonly IServiceProvider _serviceProvider;
    
    private readonly TrivistaDbContext _trivistaDbContext;

    private readonly ILogger<ConfirmMbsStatementHandler> _logger;

    public ConfirmMbsStatementHandler(IMbsService mbsService, IServiceProvider serviceProvider, TrivistaDbContext trivistaDbContext, ILogger<ConfirmMbsStatementHandler> logger)
    {
        _mbsService = mbsService;
        _serviceProvider = serviceProvider;
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
    }
    
    public async Task<Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>> Handle(ConfirmMbsStatementQuery request, CancellationToken cancellationToken)
    {
        var validator = new ConfirmMbsStatementQueryValidation();
        var exceptionResult = await TrivistaValidationException<ConfirmMbsStatementQueryValidation, ConfirmMbsStatementQuery>
            .ManageException<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>(validator, request, cancellationToken, 
                (new ConfirmStatementResponseDto(), new GetFeedbackByRequestIDResponseDto()));
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        
        var customer = await _trivistaDbContext.Customer
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        
        if(customer == null)
            return new Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>(ExceptionManager.Manage("ConfirmMbsStatement", "Customer is invalid"));
        
        var (confirmStatementResponse, getFeedbackByRequestIdResponse) = await _mbsService.ConfirmStatement(new ConfirmStatementRequestDto()
        {
            TicketNo = request.TicketNumber,
            Password = request.Password
        }, customer.MbsRequestStatementResponseCode);

        if (confirmStatementResponse.Status == "00")
        {
            if (getFeedbackByRequestIdResponse.Status == "00")
            {
                      await Task.Run( async () =>
                      {
                            var indicinaService = _serviceProvider.GetRequiredService<IIndicina>();
                            var dbContext = _serviceProvider.GetRequiredService<TrivistaDbContext>();
                            
                            var customerToUpdate = await dbContext.Customer
                                .AsNoTracking()
                                .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);

                            var jsonStatementResult = await _mbsService.GetStatementJSONObject(new GetStatementJSONObjectRequestDto()
                            {
                                TicketNo = request.TicketNumber,
                                Password = request.Password
                            });
                            
                            if (jsonStatementResult.Status == "00")
                            {
                                var indicinaResponse = await indicinaService.ProcessStatement(new BankStatementRequest()
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

                                if (string.IsNullOrEmpty(indicinaResponse))
                                {
                                    _logger.LogInformation("Unable to geed success from indicina");
                                    return;
                                }

                                customerToUpdate.SetMbsBankStatement(jsonStatementResult.Result);
                                
                                customerToUpdate.SetBankStatementAnalysis(indicinaResponse);

                                dbContext.Customer.Update(customerToUpdate);
                                
                                await dbContext.SaveChangesAsync(cancellationToken);
                                
                                _logger.LogInformation("Successful getting response from indicina");
                            }
                            else
                            {
                                _logger.LogInformation("Response from jsonStatement is: {Message}", jsonStatementResult.Message);
                            }
                      }, cancellationToken);   
                        
                      _logger.LogInformation("Successfully processed JsonStatement with Indicina");
            }
            else
            {
                _logger.LogInformation("GetFeedback failed with message: {Message}", getFeedbackByRequestIdResponse.Message);
                
                return new Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>(
                    ExceptionManager.Manage("Statement", confirmStatementResponse.Message));
            }
        }
        else
        {
            _logger.LogInformation("Confirmed statement failed with message: {Message}", confirmStatementResponse.Message);
            
            return new Result<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)>(
                ExceptionManager.Manage("Statement", confirmStatementResponse.Message));
        }

        return (confirmStatementResponse, getFeedbackByRequestIdResponse);
    }
}