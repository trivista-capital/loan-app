using MediatR;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerTransactions.EventHandlers;

public class PaymentMadeEvent: INotification
{
    public string To { get; set; }
    
    public string TransactionType { get; set; }
    
    public string Name { get; set; }
    
    public decimal Amount { get; set; }
    
    public string TransactionTime { get; set; }
    
    public string TransactionReference { get; set; }
}
        
public class PaymentMadeEventHandler: INotificationHandler<PaymentMadeEvent>
{
    private readonly IMailService _mailManager;
    
    public PaymentMadeEventHandler(IMailService mailManager)
    {
        _mailManager = mailManager;
    }

    public async Task Handle(PaymentMadeEvent notification, CancellationToken cancellationToken)
    {
        _mailManager.BuildPaymentSuccessfulMessage(notification.To, 
                                                   notification.TransactionType, 
                                                   notification.Name, 
                                                   notification.Amount, 
                                                   notification.TransactionTime, 
                                                   notification.TransactionReference);
    }
}