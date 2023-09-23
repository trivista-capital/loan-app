using MediatR;
using Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions.EventHandlers;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest.EventHandlers;

public class LoanRequestedEvent: INotification
{
    public string To { get; set; }

    public string Name { get; set; }
    
    public decimal LoanAmount { get; set; }
    
    public string Purpose { get; set; }
}

public class LoanRequestedEventHandler: INotificationHandler<LoanRequestedEvent>
{
    private readonly IMailService _mailManager;
    
    public LoanRequestedEventHandler(IMailService mailManager)
    {
        _mailManager = mailManager;
    }

    public async Task Handle(LoanRequestedEvent notification, CancellationToken cancellationToken)
    {
        _mailManager.BuildLoanRequestMessage(notification.To, notification.Name, notification.LoanAmount, notification.Purpose);
    }
}