namespace Trivista.LoanApp.ApplicationCore.Commons.Options;

public class MailOptions
{
    public string APIKey { get; set; } 
    public string From { get; set; }
    public string OTPMailTemplate { get; set; }
    public string PasswordResetMailTemplate { get; set; }
    public string LoanRequestSubject { get; set; }
    public string LoanApprovalSubject { get; set; }
    public string NewTicketRaisedSubject { get; set; }
    public string NewLoanNotificationSubject { get; set; }
    public string CustomerLoanApprovalSubject { get; set; }
    public string RepaymentSubject { get; set; }
    public string PasswordResetSubject { get; set; }
    public string WelcomeMessageSubject { get; set; }
    public string BaseAddress { get; set; }
    public string MailSubPath { get; set; } 
}