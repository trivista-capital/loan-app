using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http;


public sealed class GetSalaryHistoryRequestDto
{
    [JsonProperty("authorisationCode")]
    public long AuthorisationCode { get; set; } 
    
    [JsonProperty("firstName")]
    public string FirstName { get; set; } 
    
    [JsonProperty("lastName")]
    public string LastName { get; set; }
    
    [JsonProperty("middleName")]
    public string MiddleName { get; set; }
    
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("bankCode")]
    public string BankCode { get; set; }
    
    [JsonProperty("bvn")]
    public string Bvn { get; set; }

    [JsonProperty("authorisationChannel")]
    public string AuthorisationChannel { get; set; }
}

public sealed class GetSalaryHistoryResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; } 
    
    [JsonProperty("hasData")]
    public bool HasData { get; set; } 
    
    [JsonProperty("responseId")]
    public string ResponseId { get; set; }
    
    [JsonProperty("responseDate")]
    public string ResponseDate { get; set; }
    
    [JsonProperty("requestDate")]
    public string RequestDate { get; set; }
    
    [JsonProperty("responseCode")]
    public string ResponseCode { get; set; }
    
    [JsonProperty("responseMsg")]
    public string ResponseMsg { get; set; }
    
    [JsonProperty("data")]
    public GetSalaryHistoryResponseDataDto Data { get; set; }
    
    public sealed class GetSalaryHistoryResponseDataDto
    {
        [JsonProperty("customerId")]
        public string CustomerId { get; set; } 
    
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; } 
    
        [JsonProperty("bankCode")]
        public string BankCode { get; set; }
    
        [JsonProperty("bvn")]
        public string Bvn { get; set; }
    
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }
    
        [JsonProperty("customerName")]
        public string CustomerName { get; set; }
    
        [JsonProperty("category")]
        public string Category { get; set; }
    
        [JsonProperty("firstPaymentDate")]
        public string FirstPaymentDate { get; set; }
    
        [JsonProperty("salaryCount")]
        public string SalaryCount { get; set; }
    
        [JsonProperty("salaryPaymentDetails")]
        public List<SalaryPaymentDetailsResponseDto> SalaryPaymentDetails { get; set; }
        
        public sealed class SalaryPaymentDetailsResponseDto
        {
            [JsonProperty("paymentDate")]
            public string PaymentDate { get; set; } 
    
            [JsonProperty("amount")]
            public string Amount { get; set; } 
    
            [JsonProperty("accountNumber")]
            public string AccountNumber { get; set; }
    
            [JsonProperty("bankCode")]
            public string BankCode { get; set; }
        }
    }
}

public sealed class LoanDisbursementRequestDto
{
    [JsonProperty("customerId")]
    public string CustomerId { get; set; } 
    
    [JsonProperty("authorisationCode")]
    public string AuthorisationCode { get; set; } 
    
    [JsonProperty("AuthorisationChannel")]
    public string authorisationChannel { get; set; }
    
    [JsonProperty("phoneNumber")]
    public string PhoneNumber { get; set; }
    
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("loanAmount")]
    public string LoanAmount { get; set; }
    
    [JsonProperty("collectionAmount")]
    public string CollectionAmount { get; set; }
    
    [JsonProperty("dateOfDisbursement")]
    public string DateOfDisbursement { get; set; }
    
    [JsonProperty("dateOfCollection")]
    public string DateOfCollection { get; set; }
    
    [JsonProperty("totalCollectionAmount")]
    public string TotalCollectionAmount { get; set; }
    
    [JsonProperty("numberOfRepayments")]
    public string NumberOfRepayments { get; set; }
    
    [JsonProperty("bankCode")]
    public string BankCode { get; set; }
}

public sealed class LoanDisbursementResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; } 
    
    [JsonProperty("hasData")]
    public bool HasData { get; set; } 
    
    [JsonProperty("responseId")]
    public string ResponseId { get; set; }
    
    [JsonProperty("responseDate")]
    public string ResponseDate { get; set; }
    
    [JsonProperty("requestDate")]
    public string RequestDate { get; set; }
    
    [JsonProperty("responseCode")]
    public string ResponseCode { get; set; }
    
    [JsonProperty("responseMsg")]
    public string ResponseMsg { get; set; }
    
    [JsonProperty("data")]
    public LoanDisbursementResponseDataDto Data { get; set; }
    
    public sealed class LoanDisbursementResponseDataDto
    {
        [JsonProperty("authorisationCode")]
        public string AuthorisationCode { get; set; }
    
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }
    
        [JsonProperty("bankCode")]
        public string BankCode { get; set; }
    
        [JsonProperty("amount")]
        public string Amount { get; set; }
    
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
    
        [JsonProperty("status")]
        public string Status { get; set; }
        
        [JsonProperty("mandateReference")]
        public string MandateReference { get; set; }
    }
}


public sealed class RemittaService: IRemittaService
{
    private readonly HttpClient _client;
    
    private readonly RemittaOption _remittaOption;
    
    public RemittaService(HttpClient client, IOptions<RemittaOption> remittaOption)
    {
        _client = client;
        _remittaOption = remittaOption.Value;
    }

    public async Task<GetSalaryHistoryResponseDto> SalaryHistory(GetSalaryHistoryRequestDto model, string loanRequestId)
    {
        var authCode = new Random().Next(1000000, 900009823);
        var hash = ComputeSHA512($"{_remittaOption.APIKey}{loanRequestId}{_remittaOption.ApiToken}");

        model.AuthorisationChannel = _remittaOption.AuthorisationChannel;
        model.AuthorisationCode = authCode;

        var options = new RestClientOptions(_remittaOption.BaseUrl)
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/remita/exapp/api/v1/send/api/loansvc/data/api/v2/payday/salary/history/provideCustomerDetails", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Api_Key", _remittaOption.APIKey);
        request.AddHeader("Merchant_id", _remittaOption.MerchantId);
        request.AddHeader("Request_id", loanRequestId);
        request.AddHeader("Authorization", $"remitaConsumerKey={_remittaOption.APIKey}, remitaConsumerToken={hash}");
        var body = JsonConvert.SerializeObject(model);
        request.AddStringBody(body, DataFormat.Json);
        var jsn = JsonConvert.SerializeObject(model);
        RestResponse response = await client.ExecuteAsync(request);
        var responseBody = JsonConvert.DeserializeObject<GetSalaryHistoryResponseDto>(response.Content);
        
        return responseBody;
    }
    
    public async Task<LoanDisbursementResponseDto> DisburseLoan(LoanDisbursementRequestDto model)
    {
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("loansvc/data/api/v2/payday/post/loan", content);
        var responseBody = JsonConvert.DeserializeObject<LoanDisbursementResponseDto>(
                await response.Content.ReadAsStringAsync());
        return responseBody;
    }
    
    private static string ComputeSHA512(string input)
    {
        StringBuilder sb = new StringBuilder();
        using SHA512 sha512 = SHA512.Create();
        byte[] hashValue = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashValue).ToLower();
    }
}