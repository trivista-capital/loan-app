using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record TransferRequestDto
{
    [JsonProperty("source")]
    public string Source { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("reference")]
    public string Reference { get; set; }

    [JsonProperty("recipient")]
    public string Recipient { get; set; }
    
    [JsonProperty("reason")]
    public string Reason { get; set; }
}

public sealed record TransferResponseDto
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public TransferResponseDataDto Data { get; set; }
}

public sealed record TransferResponseDataDto
{
    [JsonProperty("integration")]
    public int Integration { get; set; }
    
    [JsonProperty("domain")]
    public string Domain { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("source")]
    public string Source { get; set; }
    
    [JsonProperty("reason")]
    public string Reason { get; set; }
    
    [JsonProperty("recipient")]
    public string Recipient { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("transfer_code")]
    public string TransferCode { get; set; }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("createdAt")]
    public string CreatedAt { get; set; }

    [JsonProperty("updatedAt")]
    public string UpdatedAt { get; set; }
}