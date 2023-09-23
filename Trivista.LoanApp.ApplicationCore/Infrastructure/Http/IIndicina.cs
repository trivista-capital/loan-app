using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http;


public sealed class LoginRequest
{
    [JsonProperty("client_id")]
    public string ClientId { get; set; }
    
    [JsonProperty("client_secret")]
    public string ClientSecret { get; set; }
}

public sealed class BankStatementRequest
{
    [JsonProperty("customer")] 
    public RequestCustomer Customer { get; set; }
    
    [JsonProperty("bankStatement")]
    public RequestBankStatement BankStatement { get; set; } = new();
    
    public sealed class RequestCustomer
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
    }
    
    public sealed class RequestBankStatement
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("content")]
        public RequestContent Content { get; set; }
    }
    
    public sealed class RequestContent
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
    }
}

public sealed class LoginResponse
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("data")]
    public LoginData Data { get; set; }
    
    public sealed class LoginData
    {
        public string Token { get; set; }
    }
}

public interface IIndicina
{
    Task<string> ProcessStatement(BankStatementRequest request);
}

public sealed class Indicina: IIndicina
{
    private readonly HttpClient _client;
    
    private readonly IndicinaOption _indicinaOption;
    
    public Indicina(HttpClient client, IOptions<IndicinaOption> indicinaOption)
    {
        _client = client;
        _indicinaOption = indicinaOption.Value;
    }

    private async Task<LoginResponse> GenerateToken()
    {
        var content = new StringContent(JsonConvert.SerializeObject(new LoginRequest()
        {
            ClientId = _indicinaOption.ClientId,
            ClientSecret = _indicinaOption.ClientSecret
        }), Encoding.UTF8, "application/json");
        
        _client.BaseAddress = new Uri(_indicinaOption.LoginUrl);
        
        var response = await _client.PostAsync("login", content);
        
        var jsonResult = await response.Content.ReadAsStringAsync();
        
        var responseBody =
            JsonConvert.DeserializeObject<LoginResponse>(jsonResult);
        
        return responseBody;
    }

    public async Task<string> ProcessStatement(BankStatementRequest request)
    {
        var token = await GenerateToken();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Data.Token);

        var str = JsonConvert.SerializeObject(request);
        
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(_indicinaOption.BaseUrl, content);
        
        var result = await response.Content.ReadAsStringAsync();
        
        if (response.IsSuccessStatusCode)
        {
            return result;
        }
        return string.Empty;
    }
}