using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Features.Account;

public class AccountVerificationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customer/{accountNumber}/{bankCode}", AccountVerificationHandler)
            .WithName("Account Verification")
            .WithTags("Customer")
            .RequireAuthorization()
            .RequireAuthorization();
    }

    private static async Task<IResult> AccountVerificationHandler(IMediator mediator, string accountNumber, string bankCode)
    {
        var response = await mediator.Send(new AccountVerificationQuery(accountNumber, bankCode));
        return response.ToOk(x => x);
    }
}

public class AccountVerificationQueryValidation : AbstractValidator<AccountVerificationQuery>
{
    public AccountVerificationQueryValidation()
    {
        RuleFor(x => x.BankCode).NotNull().NotEmpty().WithMessage("Bank code must be set");
        RuleFor(x => x.AccountNumber).NotNull().NotEmpty().WithMessage("Account number must be set");
    }
}

public sealed record AccountVerificationQuery(string AccountNumber, string BankCode): IRequest<Result<ResolveAccountDto>>;

public sealed record AccountVerificationQueryHandler: IRequestHandler<AccountVerificationQuery, Result<ResolveAccountDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly TokenManager _token;

    private readonly ILogger<AccountVerificationQueryHandler> _logger;

    private readonly IPayStackService _payStackService;
    
    public AccountVerificationQueryHandler(TrivistaDbContext trivistaDbContext, TokenManager token, ILogger<AccountVerificationQueryHandler> logger, IPayStackService payStackService)
    {
        _trivistaDbContext = trivistaDbContext;
        _token = token;
        _logger = logger;
        _payStackService = payStackService;
    }
    
    public async Task<Result<ResolveAccountDto>> Handle(AccountVerificationQuery request, CancellationToken cancellationToken)
    {
        var validator = new AccountVerificationQueryValidation();
        var exceptionResult = await TrivistaValidationException<AccountVerificationQueryValidation, AccountVerificationQuery>
            .ManageException<ResolveAccountDto>(validator, request, cancellationToken, new ResolveAccountDto());
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        
        
        var account = await _payStackService.ResolveAccount(request.AccountNumber, request.BankCode);

        return account;
    }
}