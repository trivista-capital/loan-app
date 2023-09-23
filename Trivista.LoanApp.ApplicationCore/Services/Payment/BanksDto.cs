using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public sealed record BanksDto
{
    [JsonProperty("status")]
    public bool Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("data")]
    public List<DataDto> Data { get; set; }
}

public sealed record DataDto
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("code")]
    public string Code { get; set; }
    
    [JsonProperty("slug")]
    public string Slug { get; set; }
    
    [JsonProperty("currency")]
    public string Currency { get; set; }
}