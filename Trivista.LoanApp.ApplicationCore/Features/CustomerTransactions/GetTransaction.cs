using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using EnumHelpers = Trivista.LoanApp.ApplicationCore.Commons.Helpers;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions;

public class GetTransactionController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getTransactions", GetTransactionsHandler)
            .WithName("Get Transactions")
            .WithTags("Admin");
    }

    private static async Task<IResult> GetTransactionsHandler(IMediator mediator, [FromQuery]string? startDate, [FromQuery]string? endDate, 
        [FromQuery]string? transactionType, [FromQuery]Guid? customerId, [FromQuery]int PageNumber, [FromQuery]int ItemsPerPage)
    {
        var result = await mediator.Send(new GetPaymentQuery(startDate, endDate, transactionType, customerId, PageNumber, ItemsPerPage));
        return result.ToOk(x => x);
    }
}

public record GetTransactionDto
{
    public Guid Id { get; set; }
    public string TransactionReference { get; set; }
    public decimal Amount { get; set; }
    public string Payload { get; set; }
    public string Status { get; set; }
    public bool IsSuccessful { get; set; }
    public string TransactionType { get; set; }
}

public class GetPaymentQueryValidation : AbstractValidator<GetPaymentQuery>
{
    public GetPaymentQueryValidation()
    {
        // RuleFor(x => x.startDate).NotEqual("string").NotNull().NotEmpty().WithMessage("Start date must be set");
        // RuleFor(x => x.endDate).NotEqual("string").NotNull().NotEmpty().WithMessage("End date must be set");
        // RuleFor(x => x.transactionType).NotEqual("string").NotNull().NotEmpty().WithMessage("Transaction type must be set");
    }
}

public sealed record GetPaymentQuery(string startDate, string endDate, string transactionType, Guid? CustomerId, int PageNumber, int ItemsPerPage): IRequest<Result<PaginationInfo<GetTransactionDto>>>;

public sealed class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Result<PaginationInfo<GetTransactionDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetPaymentQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<PaginationInfo<GetTransactionDto>>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        // var validator = new GetPaymentQueryValidation();
        // var exceptionResult = await TrivistaValidationException<GetPaymentQueryValidation, PaginationInfo<GetTransactionDto>>
        //     .ManageException(validator, request, cancellationToken, null);
        
        // if (!exceptionResult.IsSuccess)
        //     return exceptionResult;
        
        var transactions = _trivistaDbContext.Transaction.AsQueryable();
        
        if (!string.IsNullOrEmpty(request.startDate))
        {
            var transactionStartDate = Convert.ToDateTime(request.startDate);
            transactions = transactions.Where(x => x.Created.Date == transactionStartDate.Date);
        }
        if (!string.IsNullOrEmpty(request.endDate))
        {
            var transactionEndDate = Convert.ToDateTime(request.endDate);
            transactions = transactions.Where(x => x.Created.Date == transactionEndDate.Date);
        }
        if (!string.IsNullOrEmpty(request.transactionType))
        {
            var transactionTypeEnum = Commons.Helpers.EnumHelpers.ToEnum<TransactionType>(request.transactionType);
            transactions = transactions.Where(x => x.TransactionType == transactionTypeEnum);
        }
        if (request.CustomerId != default || request.CustomerId != null)
        {
            transactions = transactions.Include(xx=>xx.Customer)
                                       .Where(x => x.CustomerId == request.CustomerId);
        }

        var pagedResult = await PaginationData.PaginateAsync(transactions, request.PageNumber, request.ItemsPerPage);
        
        var result = await transactions.Select(x => new GetTransactionDto()
        {
            Id = x.Id,
            TransactionType = EnumHelpers.EnumHelpers.Convert(x.TransactionType),
            TransactionReference = x.TransactionReference,
            Amount = x.Amount,
            IsSuccessful = x.IsSuccessful,
            Status = EnumHelpers.EnumHelpers.Convert(x.Status)
        }).ToListAsync(cancellationToken);
        
        return new PaginationInfo<GetTransactionDto>(result, 
                                                        pagedResult.CurrentPage, 
                                                        pagedResult.PageSize, 
                                                        pagedResult.TotalItems, 
                                                        pagedResult.TotalPages);
    }
}