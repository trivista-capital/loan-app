using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record FinalTransferRequestDto
{
    [JsonProperty("transfer_code")]
    public string TransferCode { get; set; }
    
    [JsonProperty("otp")]
    public string Otp { get; set; }
}

public sealed record FinalTransferResponseDto
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public FinalTransferDataResponseDto Data { get; set; }
}

public sealed record FinalTransferDataResponseDto
{
    [JsonProperty("domain")]
    public string Domain { get; set; }
    
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("reference")]
    public string Reference { get; set; }
    
    [JsonProperty("source")]
    public string Source { get; set; }
    
    [JsonProperty("source_details")]
    public string SourceDetails { get; set; }
    
    [JsonProperty("reason")]
    public string Reason { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("failures")]
    public string Failures { get; set; }
    
    [JsonProperty("transfer_code")]
    public string TransferCode { get; set; }
    
    [JsonProperty("titan_code")]
    public string TitanCode { get; set; }
    
    [JsonProperty("transferred_at")]
    public string TransferredAt { get; set; }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("integration")]
    public int Integration { get; set; }
    
    [JsonProperty("recipient")]
    public int Recipient { get; set; }
    
    [JsonProperty("createdAt")]
    public string CreatedAt { get; set; }
    
    [JsonProperty("updatedAt")]
    public string UpdatedAt { get; set; }
}