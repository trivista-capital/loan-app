using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record TransferRecipientRequestDto
{
    [JsonProperty("type")]
    public string Type { get; set; } = "nuban";
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("bank_code")]
    public string BankCode { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
}