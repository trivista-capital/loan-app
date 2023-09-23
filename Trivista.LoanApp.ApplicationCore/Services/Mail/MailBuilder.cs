using Trivista.LoanApp.ApplicationCore.Commons.Helpers;

namespace Trivista.LoanApp.ApplicationCore.Services.Mail;

public class MailBuilder: IMailBuilder
{
    private string _message;
    private string _subject;
    private string _toEmail;
    private string _fromEmail;

    public MailBuilder WithToEmail(string toEmail)
    {
        _toEmail = toEmail;
        return this;
    }
    
    public MailBuilder WithFromEmail(string fromEmail)
    {
        _fromEmail = fromEmail;
        return this;
    }
    
    public MailBuilder BuildPaymentSuccessfulMessage(string transactionType, string name, decimal amount, string transactionTime, string transactionReference)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page6-transaction.html");
        _message = rawMailTemplate.Replace("{Transaction Type}", transactionType)
                                  .Replace("{Customer's Name}", name)
                                  .Replace("{Credit / Debit}", transactionType)
                                  .Replace("{Transaction Amount}", amount.ToString())
                                  .Replace("{Transaction Date and Time}", transactionTime)
                                  .Replace("{Transaction ID}", transactionReference);

        return this;
    }
    
    public MailBuilder BuildLoanRequestMessage(string name, decimal loanAmount, string loanPurpose)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page7-loan-request.html");
        _message = rawMailTemplate.Replace("{Customer's Name}", name)
            .Replace("{Requested Loan Amount}", loanAmount.ToString())
            .Replace("{Customer's Stated Loan Purpose}", loanPurpose);

        return this;
    }

    public MailBuilder BuildLoanApprovedByAdminMessage(string name, decimal loanAmount, int loanTenure, decimal interestRate)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page8-loan-approved-by-admin.html");
        _message = rawMailTemplate.Replace("{Customer's Name}", name)
            .Replace("{Approved Loan Amount}", loanAmount.ToString())
            .Replace("{Approved Loan Tenure}", loanTenure.ToString())
            .Replace("{Approved Interest Rate}", interestRate.ToString());

        return this;
    }
    
    public MailBuilder BuildBuildNewTicketRaisedMessage(string adminName, Guid ticketId, string customerName, DateTime dateTimeOfTicket, string issueCategory, string descriptionOfIssue)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page12-ticket-raised.html");
        _message = rawMailTemplate.Replace("{Admin's Name}", adminName)
            .Replace("{Ticket ID}", ticketId.ToString())
            .Replace("{Customer's Name}", customerName)
            .Replace("{Ticket Date and Time}", dateTimeOfTicket.ToString())
            .Replace("{Category of the Issue}", issueCategory)
            .Replace("{Brief Description}", descriptionOfIssue);

        return this;
    }
    
    public MailBuilder BuildNewLoanNotificationMessage(string adminName, string customerName, decimal loanAmount, string loanPurpose, DateTime dateTimeOfTicket)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page14-new-loan-request-to-admin.html");
        _message = rawMailTemplate.Replace("{Admin's Name}", adminName)
            .Replace("{Customer's Name}", customerName)
            .Replace("{Requested Loan Amount}", loanAmount.ToString())
            .Replace("{Stated Loan Purpose}", loanPurpose)
            .Replace("{Requested Date and Time}", dateTimeOfTicket.ToString());

        return this;
    }

    public MailBuilder BuildCustomerLoanApprovalNotificationMessage(string adminName, string customerName,
        decimal loanAmount, decimal interestRate, int loanTenure, string repaymentScheduleType)
    {
        var rawMailTemplate = FileHelper.ExtractMailTemplate("page9-loan-approved-by-customer-and-disbursed.html");
        _message = rawMailTemplate.Replace("{Admin's Name}", adminName)
            .Replace("{Customer's Name}", customerName)
            .Replace("{Approved Loan Amount}", loanAmount.ToString())
            .Replace("{Interest Rate}", interestRate.ToString())
            .Replace("{Loan Tenure}", loanTenure.ToString())
            .Replace("{Monthly/Weekly}", repaymentScheduleType);

        return this;
    }

    public MailBuilder WithSubject(string otpSubject)
    {
        _subject = otpSubject;
        return this;
    }

    public MailObject BuildMailDto()
    {
        return new MailObject()
        {
            BodyAmp = _message,
            CharSet = "utf-8",
            From = _fromEmail,
            IsTransactional = true,
            To = _toEmail,
            Sender = _fromEmail,
            Subject = _subject
        };
    }

    public static implicit operator MailObject(MailBuilder builder)
    {
        return builder.BuildMailDto();
    }
}