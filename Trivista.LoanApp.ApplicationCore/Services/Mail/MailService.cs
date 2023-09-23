using System.Diagnostics;
using System.Text;
using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.ApplicationCore.Services.Mail;

public class MailService: IMailService
{
    private static MailOptions _mailOptions;
    private readonly HttpClient _client;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<MailOptions> mailOptions, HttpClient client, ILogger<MailService> logger)
    {
        _client = client;
        _logger = logger;
        _mailOptions = mailOptions.Value;
    }
    
    public void BuildPaymentSuccessfulMessage(string to, string transactionType, string name, decimal amount, string transactionTime, string transactionReference)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(to)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.RepaymentSubject)
            .BuildPaymentSuccessfulMessage(transactionType, name, amount,  transactionTime,  transactionReference)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }
    
    public void BuildLoanRequestMessage(string to, string name, decimal loanAmount, string loanPurpose)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(to)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.LoanRequestSubject)
            .BuildLoanRequestMessage(name,loanAmount, loanPurpose)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }
    
    public void BuildLoanApprovedByAdminMessage(string to, string name, decimal loanAmount, int loanTenure, decimal interestRate)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(to)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.LoanApprovalSubject)
            .BuildLoanApprovedByAdminMessage(name,loanAmount, loanTenure, interestRate)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }
    
    public void BuildBuildNewTicketRaisedMessage(string to, string adminName, Guid ticketId, string customerName, DateTime dateTimeOfTicket, string issueCategory, string descriptionOfIssue)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(to)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.NewTicketRaisedSubject)
            .BuildBuildNewTicketRaisedMessage(adminName,ticketId, customerName, dateTimeOfTicket, issueCategory, descriptionOfIssue)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }
    
    public void BuildNewLoanNotificationMessage(string to, string adminName, string customerName, decimal loanAmount, string loanPurpose, DateTime dateTimeOfTicket)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(to)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.NewLoanNotificationSubject)
            .BuildNewLoanNotificationMessage(adminName,  customerName, loanAmount, loanPurpose, dateTimeOfTicket)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }

    public void BuildCustomerLoanApprovalNotificationMessage(string adminEmail, string adminName, string customerName,
        decimal loanAmount, decimal interestRate, int loanTenure, string repaymentScheduleType)
    {
        var builder = new MailBuilder();
        var mailObject = builder.WithToEmail(adminEmail)
            .WithFromEmail(_mailOptions.From)
            .WithSubject(_mailOptions.CustomerLoanApprovalSubject)
            .BuildCustomerLoanApprovalNotificationMessage(adminName,  customerName, loanAmount,  interestRate, loanTenure,  repaymentScheduleType)
            .BuildMailDto(); 
        SendEmailAsync(mailObject);
    }

    private void SendEmail(MailObject dto)
    { 
        Configuration config = new Configuration();
        // Configure API key authorization: apikey
        config.ApiKey.Add("X-ElasticEmail-ApiKey", _mailOptions.APIKey);
        var apiInstance = new EmailsApi(config);
        var to = new List<string> { dto.To };
        var recipients = new TransactionalRecipient(to: to);
        EmailTransactionalMessageData emailData = new EmailTransactionalMessageData(recipients: recipients)
        {
            Content = new EmailContent
            {
                Body = new List<BodyPart>()
            }
        };
        BodyPart htmlBodyPart = new BodyPart
        {
            ContentType = BodyContentType.HTML,
            Charset = "utf-8",
            Content = dto.BodyAmp
        };
        BodyPart plainTextBodyPart = new BodyPart
        {
            ContentType = BodyContentType.PlainText,
            Charset = "utf-8",
            Content = dto.BodyAmp
        };
        emailData.Content.Body.Add(htmlBodyPart);
        emailData.Content.Body.Add(plainTextBodyPart);
        emailData.Content.From = dto.From;
        emailData.Content.Subject = dto.Subject;

        try
        {
            // Send Bulk Emails
            _logger.LogInformation("ABout sending mail");
            var result = apiInstance.EmailsTransactionalPost(emailData);
            _logger.LogInformation("Mail sent ith ID {@ID}, sent", result.MessageID);
        }
        catch (ApiException  ex)
        {
            _logger.LogError(ex, "Exception when calling EmailsApi.EmailsPost: " + ex.Message);
            _logger.LogError(ex,  "Status Code: " + ex.ErrorCode);
            _logger.LogError(ex, "An error occured, see stack trace: {@StackTrace}", ex.StackTrace);
        }
    }
    
    private async Task SendEmailAsync(MailObject model)
    {
        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            // Send Bulk Emails
            _logger.LogInformation("Sending email");
            var result = _client.PostAsync("/sendMail", content).GetAwaiter().GetResult();
            if (result.IsSuccessStatusCode)
            {
                var con = result.Content.ReadAsStringAsync();
            }
        }
        catch (ApiException  ex)
        {
            _logger.LogError(ex, "Exception when calling EmailsApi.EmailsPost: {@Message}",  ex.Message);
            _logger.LogInformation("Status Code: {@StatusCode} ", ex.ErrorCode);
            _logger.LogInformation(ex.StackTrace);
        }
    }
}