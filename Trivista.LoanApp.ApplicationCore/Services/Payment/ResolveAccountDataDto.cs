using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record ResolveAccountDataDto
{
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("account_name")]
    public string AccountName { get; set; }
    
    [JsonProperty("bank_id")]
    public int BankId { get; set; }
}