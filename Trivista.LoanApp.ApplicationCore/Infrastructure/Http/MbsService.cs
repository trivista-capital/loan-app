using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

public class RequestStatementRequestDto
{
    [JsonProperty("accountNo")]
    public string AccountNo { get; set; }
    
    [JsonProperty("bankId")]
    public int BankId { get; set; }
    
    [JsonProperty("destinationId")]
    public int DestinationId { get; set; }
    
    [JsonProperty("startDate")]
    public string StartDate { get; set; }
    
    [JsonProperty("endDate")]
    public string EndDate { get; set; }
    
    [JsonProperty("role")]
    public string Role { get; set; }
    
    [JsonProperty("username")]
    public string Username { get; set; }
    
    [JsonProperty("country")]
    public string Country { get; set; }
    
    [JsonProperty("phone")]
    public string Phone { get; set; }
    
    [JsonProperty("applicants")]
    public List<RequestStatementApplicantRequestDto> Applicants { get; set; }
}

public class RequestStatementApplicantRequestDto
{
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("applicationNo")]
    public string ApplicationNo { get; set; }
}

public class RequestStatementResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("result")]
    public int Result { get; set; }
}

public class ConfirmStatementRequestDto
{
    [JsonProperty("ticketNo")]
    public string TicketNo { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
}

public class ConfirmStatementResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
}

public class GetFeedbackByRequestIDRequestDto
{
    [JsonProperty("requestId")]
    public int RequestId { get; set; }
}

public class GetFeedbackByRequestIDResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("result")]
    public GetFeedbackByRequestIDResultDto Result { get; set; }
}

public class GetFeedbackByRequestIDResultDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("feedback")]
    public string Feedback { get; set; }
}

public class GetStatementJSONObjectRequestDto
{
    [JsonProperty("ticketNo")]
    public string TicketNo { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }
}

public class GetStatementJSONObjectResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("result")]
    public string Result { get; set; }
}

public class SelectActiveRequestBanksJSONObjectResponseDto
{
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("result")]
    public List<SelectActiveResponseBanksJSONObjectResponseDto> Result { get; set; }
}

public class SelectActiveResponseBanksJSONObjectResponseDto
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("name")]
    public string Name { get; set; }
    
    [JsonProperty("sortCode")]
    public string SortCode { get; set; }
}

public interface IMbsService
{
    Task<RequestStatementResponseDto> RequestStatement(RequestStatementRequestDto model);
    Task<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)> ConfirmStatement(ConfirmStatementRequestDto model, int requestId);
    Task<GetStatementJSONObjectResponseDto> GetStatementJSONObject(GetStatementJSONObjectRequestDto model);

    Task<SelectActiveRequestBanksJSONObjectResponseDto> SelectActiveRequestBanks();
}

public class MbsService: IMbsService
{
    private readonly HttpClient _client;
    
    private readonly MbsOption _mbsOption;
    
    public MbsService(HttpClient client, IOptions<MbsOption> mbsOption)
    {
        _client = client;
        _mbsOption = mbsOption.Value;
    }
    
    public async Task<RequestStatementResponseDto> RequestStatement(RequestStatementRequestDto model)
    {
        var req = JsonConvert.SerializeObject(model);
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("RequestStatement", content);
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var serializedJsonResponseBody = JsonConvert.DeserializeObject<RequestStatementResponseDto>(jsonResponse);
        return serializedJsonResponseBody;
    }
    
    public async Task<(ConfirmStatementResponseDto, GetFeedbackByRequestIDResponseDto)> ConfirmStatement(ConfirmStatementRequestDto model, int requestId)
    {
        GetFeedbackByRequestIDResponseDto feedbackBody = default;
        
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("ConfirmStatement", content);
        var confirmStatementBody = JsonConvert.DeserializeObject<ConfirmStatementResponseDto>(await response.Content.ReadAsStringAsync());
        if (confirmStatementBody.Status == "00")
        {
            await Task.Delay(10000);
            feedbackBody = await GetFeedbackByRequestID(new GetFeedbackByRequestIDRequestDto(){RequestId = requestId});
            if (feedbackBody.Status == "00")
            {
                return (confirmStatementBody, feedbackBody);
            }
        }
        return (confirmStatementBody, feedbackBody);
    }
    
    private async Task<GetFeedbackByRequestIDResponseDto> GetFeedbackByRequestID(GetFeedbackByRequestIDRequestDto model)
    {
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("GetFeedbackByRequestID", content);
        var res = await response.Content.ReadAsStringAsync();
        var responseBody =
            JsonConvert.DeserializeObject<GetFeedbackByRequestIDResponseDto>(
                await response.Content.ReadAsStringAsync());
        return responseBody;
    }
    
    public async Task<GetStatementJSONObjectResponseDto> GetStatementJSONObject(GetStatementJSONObjectRequestDto model)
    {
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("GetStatementJSONObject", content);
        var responseBody =
            JsonConvert.DeserializeObject<GetStatementJSONObjectResponseDto>(
                await response.Content.ReadAsStringAsync());
        return responseBody;
    }
    
    public async Task<SelectActiveRequestBanksJSONObjectResponseDto> SelectActiveRequestBanks()
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("SelectActiveRequestBanks", content);
        var responseBody =
            JsonConvert.DeserializeObject<SelectActiveRequestBanksJSONObjectResponseDto>(
                await response.Content.ReadAsStringAsync());
        return responseBody;
    }
}