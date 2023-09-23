using MediatR;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public class NewTicketRaisedEvent: INotification
{
    public string To { get; set; }

    public string AdminName { get; set; }
    
    public Guid TicketId { get; set; }
    
    public string CustomerName { get; set; }
    
    public DateTime DateAndTimeOfTicket { get; set; }
    
    public string IssueCategory { get; set; }
    
    public string DescriptionOfIssue { get; set; }
}

public class NewTicketRaisedEventEventHandler: INotificationHandler<NewTicketRaisedEvent>
{
    private readonly IMailService _mailManager;
    
    public NewTicketRaisedEventEventHandler(IMailService mailManager)
    {
        _mailManager = mailManager;
    }

    public async Task Handle(NewTicketRaisedEvent notification, CancellationToken cancellationToken)
    {
        _mailManager.BuildBuildNewTicketRaisedMessage(notification.To, notification?.AdminName, notification.TicketId, notification.CustomerName, notification.DateAndTimeOfTicket, 
                                notification.IssueCategory, notification.DescriptionOfIssue);
    }
}