using MediatR;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class NewLoanRequestNotificationEvent: INotification
{
    public string To { get; set; }
    
    public string AdminName { get; set; }
    
    public string CustomerName { get; set; }
    
    public decimal LoanAmount { get; set; }
    
    public string LoanPurpose { get; set; }
    
    public DateTime DateTimeOfRequest { get; set; }
}

public class NewLoanRequestNotificationEventHandler: INotificationHandler<NewLoanRequestNotificationEvent>
{
    private readonly IMailService _mailManager;
    
    public NewLoanRequestNotificationEventHandler(IMailService mailManager)
    {
        _mailManager = mailManager;
    }

    public async Task Handle(NewLoanRequestNotificationEvent notification, CancellationToken cancellationToken)
    {
        _mailManager.BuildNewLoanNotificationMessage(notification.To, 
                                                     notification.AdminName, 
                                                     notification.CustomerName, 
                                                     notification.LoanAmount, 
                                                     notification.LoanPurpose, 
                                                     notification.DateTimeOfRequest);
    }
}