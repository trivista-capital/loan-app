namespace Trivista.LoanApp.ApplicationCore.Services.Mail;

public interface IMailService
{
    void BuildPaymentSuccessfulMessage(string to, string transactionType, string name, decimal amount, string transactionTime, string transactionReference);
    void BuildLoanRequestMessage(string to, string name, decimal loanAmount, string loanPurpose);

    void BuildLoanApprovedByAdminMessage(string to, string name, decimal loanAmount, int loanTenure,
        decimal interestRate);

    void BuildBuildNewTicketRaisedMessage(string to, string adminName, Guid ticketId, string customerName,
        DateTime dateTimeOfTicket, string issueCategory, string descriptionOfIssue);

    void BuildNewLoanNotificationMessage(string to, string adminName, string customerName, decimal loanAmount,
        string loanPurpose, DateTime dateTimeOfTicket);
    
    void BuildCustomerLoanApprovalNotificationMessage(string adminEmail, string adminName, string customerName, decimal loanAmount,
        decimal interestRate, int loanTenure, string repaymentScheduleType);
}