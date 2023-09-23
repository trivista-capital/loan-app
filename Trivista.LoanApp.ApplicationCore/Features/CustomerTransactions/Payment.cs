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
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions.EventHandlers;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions;

public class PaymentController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/makePayment", MakePaymentHandler)
            .WithName("Make Payment")
            .WithTags("Customer");
    }

    private static async Task<IResult> MakePaymentHandler(IMediator mediator, [FromBody]AddPaymentCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToOk(x => x);
    }
}

public class AddPaymentCommandValidation : AbstractValidator<AddPaymentCommand>
{
    public AddPaymentCommandValidation()
    {
        RuleFor(x => x.Email).NotEqual("string").NotNull().NotEmpty().WithMessage("Email must be set");
        RuleFor(x => x.TransactionReference).NotEqual("string").NotNull().NotEmpty().WithMessage("Transaction reference must be set");
        RuleFor(x => x.Payload).NotEqual("string").NotNull().NotEmpty().WithMessage("Payload must be set");
        RuleFor(x => x.RepaymentScheduleId).Must(BeAvalidGuid).NotNull().NotEmpty().WithMessage("Dob must be set");
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be a positive value");
        // RuleFor(x => x.Amount).Must((command, amount) =>
        // {
        //     var repaymentScheduleAmount = _trivistaDbContext.RepaymentSchedule
        //         .Where(x => x.Id == command.RepaymentScheduleId)
        //         .Select(x => x.Amount).FirstOrDefault();
        //     if (amount < repaymentScheduleAmount)
        //         return false;
        //     return true;
        // }).WithMessage("Amount must be a positive value");
    }
    private bool BeAvalidGuid(Guid repaymentScheduleId)
    {
        if (repaymentScheduleId.Equals(default))
            return false;
        return true;
    }
}

public sealed record AddPaymentCommand(string Email, string TransactionReference, decimal Amount, string Payload, Guid RepaymentScheduleId): IRequest<Result<bool>>;

public sealed class AddPaymentCommandHandler : IRequestHandler<AddPaymentCommand, Result<bool>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly IPublisher _publisher;
    
    public AddPaymentCommandHandler(TrivistaDbContext trivistaDbContext, IPublisher publisher)
    {
        _trivistaDbContext = trivistaDbContext;
        _publisher = publisher;
    }
    
    public async Task<Result<bool>> Handle(AddPaymentCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddPaymentCommandValidation();
        var exceptionResult = await TrivistaValidationException<AddPaymentCommandValidation, AddPaymentCommand>
            .ManageException<bool>(validator, request, cancellationToken, true);
        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        var customer = await 
            _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        
        var repaymentSchedule = await _trivistaDbContext.RepaymentSchedule
            .Where(x => x.Id == request.RepaymentScheduleId)
            .Select(x => x).FirstAsync(cancellationToken);
        
        if(repaymentSchedule is null)
            return new Result<bool>(ExceptionManager.Manage("Transaction", "Unable to get repayment schedule"));
        
        // if(request.Amount < repaymentSchedule.Amount)
        //     return new Result<bool>(ExceptionManager.Manage("Transaction", "Amount can not be lesser than repayment amount"));

        var loanRequest = await _trivistaDbContext.LoanRequest.Where(x => x.Id == repaymentSchedule.LoanRequestId)
                             .FirstOrDefaultAsync(cancellationToken);

        var loanBalance = loanRequest.LoanDetails.LoanBalance - repaymentSchedule.RepaymentAmount;
            
        loanRequest.SetLoanBalance(loanBalance);
        
        var paymentId = Guid.NewGuid();
        
        var payment = Transaction.Factory.Build(paymentId, request.TransactionReference, request.Amount,
                                request.Payload, RepaymentStatus.Paid, true, TransactionType.Repayment, loanRequest.Id)
                                .SetCustomer(customer).SetReschedulePayment(repaymentSchedule);

        loanRequest.SetLoanStatus(loanBalance);
        
        repaymentSchedule.UpdateRepaymentStatus();

        _trivistaDbContext.RepaymentSchedule.Update(repaymentSchedule);
        
        _trivistaDbContext.LoanRequest.Update(loanRequest);
        
        await _trivistaDbContext.Transaction.AddAsync(payment, cancellationToken);
        
        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (result <= 0)
            return new Result<bool>(ExceptionManager.Manage("Transaction", "Unable to complete repayment"));
        
        _publisher.Publish(new PaymentMadeEvent()
        {
            Name = $"{customer.FirstName} {customer.LastName}",
            Amount = request.Amount,
            To = request.Email,
            TransactionReference = "12345",
            TransactionTime = DateTime.Now.ToString(),
            TransactionType = TransactionType.Repayment.ToString()
        });
        return true;
    }
    
    
}