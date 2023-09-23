using MediatR;
using Trivista.LoanApp.ApplicationCore.Services.Mail;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class LoanDisbursedEvent: INotification
{
    public string AdminEmail { get; set; }
    
    public string AdminName { get; set; }
    
    public string CustomerName { get; set; }
    
    public decimal LoanAmount { get; set; }
    
    public decimal InterestRate { get; set; }
    
    public int LoanTenure { get; set; }
    
    public string RepaymentScheduleType { get; set; }
}

public sealed class LoanDisbursedEventHandler : INotificationHandler<LoanDisbursedEvent>
{
    private readonly IMailService _mailService;
    
    public LoanDisbursedEventHandler(IMailService mailService)
    {
        _mailService = mailService;
    }

    public async Task Handle(LoanDisbursedEvent notification, CancellationToken cancellationToken)
    {
         _mailService.BuildCustomerLoanApprovalNotificationMessage(notification.AdminEmail, notification.AdminName,
            notification.CustomerName,
            notification.LoanAmount, notification.InterestRate, notification.LoanTenure,
            notification.RepaymentScheduleType);
    }
}