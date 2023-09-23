using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.Customer;
using Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker.Messages;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker.Consumers;

public sealed class CreateCustomerConsumer: IConsumer<CreateCustomerMessage>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateCustomerConsumer> _logger;

    public CreateCustomerConsumer(IMediator mediator, ILogger<CreateCustomerConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<CreateCustomerMessage> context)
    {
        var response = await _mediator.Send(new AddCustomerCommand(Guid.Parse(context.Message.Id), context.Message.FirstName,
            context.Message.MiddleName, context.Message.LastName,
            context.Message.Email, context.Message.PhoneNumber, context.Message.Sex, context.Message.Dob, 
            context.Message.RoleId, context.Message.Address, context.Message.UserType));
        response.Match<bool>(x =>
        {
            var response = x;
            _logger.LogInformation("Customer created successfully");
            return true;
        }, exception =>
        {
            if (exception is ValidationException validationException)
            {
                var errorArrays = validationException.Errors.Select(x => x.ErrorMessage).ToArray();
                var errors = string.Join("", errorArrays);
                _logger.LogError(errors);
                return false;
            }
            _logger.LogError(exception, exception.Message);
            return false;
        });
    }
}