using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record ResolveAccountDto
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public ResolveAccountDataDto Data { get; set; }
}