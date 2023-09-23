namespace Trivista.LoanApp.ApplicationCore.Services.Mail;

public interface IMailBuilder
{
    MailBuilder WithToEmail(string toEmail);
    MailBuilder WithFromEmail(string fromEmail);
    
    MailBuilder WithSubject(string subject);

    MailBuilder BuildPaymentSuccessfulMessage(string transactionType, string name, decimal amount,
        string transactionTime, string transactionReference);

    MailBuilder BuildLoanRequestMessage(string name, decimal loanAmount, string loanPurpose);

    MailBuilder BuildLoanApprovedByAdminMessage(string name, decimal loanAmount, int loanTenure, decimal interestRate);

    MailBuilder BuildBuildNewTicketRaisedMessage(string adminName, Guid ticketId, string customerName,
        DateTime dateTimeOfTicket, string issueCategory, string descriptionOfIssue);

    MailBuilder BuildNewLoanNotificationMessage(string adminName, string customerName, decimal loanAmount,
        string loanPurpose, DateTime dateTimeOfTicket);

    MailBuilder BuildCustomerLoanApprovalNotificationMessage(string adminName, string customerName,
        decimal loanAmount, decimal interestRate, int loanTenure, string repaymentScheduleType);

    MailObject BuildMailDto();
}