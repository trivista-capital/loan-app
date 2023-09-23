using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record TransferRecipientResponseDto
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public TransferRecipientDataResponseDto Data { get; set; }
}

public sealed record TransferRecipientDataResponseDto
{
    [JsonProperty("status")]
    public bool Active { get; set; }
    
    [JsonProperty("createdAt")]
    public string CreatedAt { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
    
    [JsonProperty("domain")]
    public string Domain { get; set; }
    
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("integration")]
    public int Integration { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("recipient_code")]
    public string RecipientCode { get; set; }
    
    [JsonProperty("type")]
    public string Type { get; set; }
    
    [JsonProperty("updatedAt")]
    public string UpdatedAt { get; set; }
    
    [JsonProperty("is_deleted")]
    public bool IsDeleted { get; set; }
    
    [JsonProperty("details")]
    public TransferRecipientDetailsResponseDto Details { get; set; }
}

public sealed record TransferRecipientDetailsResponseDto
{
    [JsonProperty("authorization_code")]
    public string AuthorizationCode { get; set; }
    
    [JsonProperty("account_number")]
    public string AccountNumber { get; set; }
    
    [JsonProperty("account_name")]
    public string AccountName { get; set; }
    
    [JsonProperty("bank_code")]
    public string BankCode { get; set; }
    
    [JsonProperty("bank_name")]
    public string BankName { get; set; }
}