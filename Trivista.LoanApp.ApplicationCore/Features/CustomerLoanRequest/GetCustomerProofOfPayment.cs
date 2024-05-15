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
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Specifications;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class GetCustomerProofOfPaymentController: ICarterModule
{
     public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/loanRequests/ProofOfPayment/{id}", HandleProofOfPaymentRequest)
            .WithName("ProofOfPayment")
            .RequireAuthorization()
            .WithTags("Loan Request");
    }
    
    private async Task<IResult> HandleProofOfPaymentRequest(IMediator mediator, [FromQuery]Guid id)
    {
        var result = await mediator.Send(new ProofOfPaymentRequestQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record GetProofOfPaymentRequestDto
{
    public Guid Id { get; set; }
    public string ProofOFAddressFileName { get; set; }
    
    public string ProofOFAddressFileType { get; set; }
    
    public long ProofOFAddressFileLength { get; set; }
    public string ProofOFAddressFile { get; set; }

    public static explicit operator GetProofOfPaymentRequestDto(LoanRequest loanRequest)
    {
        return new GetProofOfPaymentRequestDto()
        {
            Id = loanRequest.Id,
            ProofOFAddressFileName = loanRequest.ProofOfAddress.ProofOFAddressFileName, //Required
            ProofOFAddressFileType = loanRequest.ProofOfAddress.ProofOFAddressFileType,
            ProofOFAddressFileLength = loanRequest.ProofOfAddress.ProofOFAddressFileLength, //Required
            ProofOFAddressFile = loanRequest.ProofOfAddress.ProofOFAddressFile, //Required
        };
    }
}

public class ProofOfPaymentRequestQueryValidation : AbstractValidator<ProofOfPaymentRequestQuery>
{
    public ProofOfPaymentRequestQueryValidation()
    {
        //LoanRequest Id validation
        RuleFor(x => x.Id).NotEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"))
            .NotNull().NotEmpty().WithMessage("Loan request Id is invalid");
    }
}
public sealed record ProofOfPaymentRequestQuery(Guid Id): IRequest<Result<GetProofOfPaymentRequestDto>>;

public sealed class ProofOfPaymentRequestQueryHandler : IRequestHandler<ProofOfPaymentRequestQuery, Result<GetProofOfPaymentRequestDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public ProofOfPaymentRequestQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<GetProofOfPaymentRequestDto>> Handle(ProofOfPaymentRequestQuery request, CancellationToken cancellationToken)
    {
        var predicate = CustomerLoanRequestSpecification.WhereId(request.Id);
        var loanRequestFromDb = await _trivistaDbContext.LoanRequest.FirstOrDefaultAsync(predicate, cancellationToken);
        if(loanRequestFromDb == null || loanRequestFromDb.ProofOfAddress == null)
            return new Result<GetProofOfPaymentRequestDto>(ExceptionManager.Manage("Customer", "Customer has not uploaded proof of address"));
        var loanRequest = (GetProofOfPaymentRequestDto)loanRequestFromDb;
        return loanRequest;
    }
}