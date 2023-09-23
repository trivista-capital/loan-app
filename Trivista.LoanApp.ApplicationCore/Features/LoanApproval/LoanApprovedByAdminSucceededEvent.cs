using MediatR;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class LoanApprovedByAdminSucceededEvent: INotification
{
    public string To { get; set; }

    public string Name { get; set; }
    
    public decimal LoanAmount { get; set; }
    
    public int LoanTenure { get; set; }
    
    public decimal InterestRate { get; set; }
}

public class LoanApprovedByAdminSucceededEventHandler: INotificationHandler<LoanApprovedByAdminSucceededEvent>
{
    private readonly IMailService _mailManager;
    
    public LoanApprovedByAdminSucceededEventHandler(IMailService mailManager)
    {
        _mailManager = mailManager;
    }

    public async Task Handle(LoanApprovedByAdminSucceededEvent notification, CancellationToken cancellationToken)
    {
        _mailManager.BuildLoanApprovedByAdminMessage(notification.To, notification.Name, notification.LoanAmount, notification.LoanTenure, notification.InterestRate);
    }
}