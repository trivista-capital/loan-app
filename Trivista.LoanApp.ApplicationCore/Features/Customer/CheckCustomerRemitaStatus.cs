using System.Text.RegularExpressions;
using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class CheckCustomerRemitaStatusController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/customer/checkRemitaStatus", CheckRemitaStatusHandler)
            .WithName("Verify Remita Status")
            .WithTags("Customer");
    }
    
    private async Task<IResult> CheckRemitaStatusHandler(IMediator mediator, CheckRemitaStatusQuery model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x => x);
    }
}

public class CheckRemitaStatusQueryValidation: AbstractValidator<CheckRemitaStatusQuery>
{
    public CheckRemitaStatusQueryValidation()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("FirstName must be set");
        RuleFor(x => x.MiddleName).NotNull().NotEmpty().WithMessage("MiddleName must be set");
        RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("LastName must be set");
        RuleFor(x => x.AccountNumber).NotNull().NotEmpty().WithMessage("Account number must be set");
        RuleFor(x => x.Bvn).NotNull().NotEmpty().WithMessage("Bvn must be set");
        RuleFor(x => x.BankName).NotNull().NotEmpty().WithMessage("Bank Name must be set");
    }
}

public sealed record CheckRemitaStatusQuery(string FirstName, string LastName, string MiddleName, string AccountNumber, string Bvn, string BankName) : IRequest<Result<bool>>;

public sealed class CheckRemitaStatusHandler : IRequestHandler<CheckRemitaStatusQuery, Result<bool>>
{
    private readonly IRemittaService _remittaService;
    
    private readonly IPayStackService _payStackService;
    
    private readonly IMbsService _mbsService;

    private readonly TrivistaDbContext _trivistaDbContext;

    public CheckRemitaStatusHandler(IMbsService mbsService, TrivistaDbContext trivistaDbContext, IRemittaService remittaService, IPayStackService payStackService)
    {
        _mbsService = mbsService;
        _trivistaDbContext = trivistaDbContext;
        _remittaService = remittaService;
        _payStackService = payStackService;
    }
    
    public async Task<Result<bool>> Handle(CheckRemitaStatusQuery request, CancellationToken cancellationToken)
    {
        var validator = new CheckRemitaStatusQueryValidation();
        var exceptionResult = await TrivistaValidationException<CheckRemitaStatusQueryValidation, CheckRemitaStatusQuery>
            .ManageException<bool>(validator, request, cancellationToken, false);
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        //Call mbs for banks  
        var banksService = await _payStackService.GetBanks();
        if(banksService == null)
            return new Result<bool>(ExceptionManager.Manage("Loan Request", "Unable to retrieve banks at the moment"));

        var bank = banksService.Data.Where(x => x.Name.ToLower().Trim() == request.BankName.ToLower().Trim()).Select(x=>x).FirstOrDefault();
        if(bank == null)
            return new Result<bool>(ExceptionManager.Manage("Loan Request", "Unable to verify customer bank"));
            
        //Call Remitta
        var remittaMandateResponse = await _remittaService.SalaryHistory(new GetSalaryHistoryRequestDto()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            AccountNumber = request.AccountNumber,
            BankCode = bank.Code,
            Bvn = request.Bvn
        }, Guid.NewGuid().ToString());

        return remittaMandateResponse switch
        {
            { HasData: true } when remittaMandateResponse.Status.ToUpper() == "success".ToUpper() &&
                                   remittaMandateResponse.ResponseMsg == "SUCCESS" => remittaMandateResponse.Data
                .SalaryPaymentDetails.Any(),
            { HasData: false } when remittaMandateResponse.Status == null || remittaMandateResponse.Status.ToUpper() == "fail".ToUpper() &&
                                    remittaMandateResponse.ResponseMsg != "SUCCESS" => new Result<bool>(new TrivistaException("Not a remita user", 404)),
            _ => new Result<bool>(ExceptionManager.Manage("Statement", "Unable to determine remita status"))
        };
    }
}

