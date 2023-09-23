using System.Security.Cryptography;
using System.Text;
using LanguageExt.ClassInstances.Pred;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Features.Integrations;
using Trivista.LoanApp.ApplicationCore.Interfaces;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

public class SmileIdService: ISmileIdService
{
    private readonly ILogger<SmileIdService> _logger;
    private readonly HttpClient _client;
    private readonly IOptionsMonitor<SmileIdOption> _optionsMonitor;
    
    public SmileIdService(HttpClient client, ILogger<SmileIdService> logger, IOptionsMonitor<SmileIdOption> optionsMonitor)
    {
        _client = client;
        _logger = logger;
        _optionsMonitor = optionsMonitor;
    }
    
    public async Task<CustomerPersonalDetailsResponseDto> VerifyUserDetails(GetCustomerPersonalDetailsRequestDto request, string timeStamp, Guid paramsId)
    {
        try
        {
            request.Country = _optionsMonitor.CurrentValue.Country;
            request.IdType = _optionsMonitor.CurrentValue.IdType;
            request.Signature = Signature.Generate(_optionsMonitor.CurrentValue.SandBoxApiKey, _optionsMonitor.CurrentValue.PartnerId, timeStamp);
            request.Source_Sdk = _optionsMonitor.CurrentValue.Source_Sdk;
            request.Source_Sdk_Version = _optionsMonitor.CurrentValue.Source_Sdk_Version;
            request.Partner_id = _optionsMonitor.CurrentValue.PartnerId;
            request.First_Name = request.First_Name;
            request.Last_Name = request.Last_Name;
            request.CallBack_Url = _optionsMonitor.CurrentValue.Call_Back_Url;
            request.PhoneNumber = request.PhoneNumber;
            request.PartnerParams = new PartnerParams()
            {
              User_Id   = $"{paramsId}",
              JobId = $"{paramsId}",
              Job_Type = _optionsMonitor.CurrentValue.Job_Type
            };
            var serialized = JsonConvert.SerializeObject(request);
            var body = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var httpResult = await _client.PostAsync("id_verification", body);
            if (httpResult.IsSuccessStatusCode)
            {
                var result = await httpResult.Content.ReadAsStringAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<CustomerPersonalDetailsResponseDto>(result);
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", JsonConvert.DeserializeObject(await httpResult.Content.ReadAsStringAsync()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }

        return null;
    }
}


public sealed class Signature
{
    public static string Generate(string apiKey, string partnerId, string timestamp)
    {
        string timeStamp = timestamp;
        string data = timeStamp + partnerId + "sid_request";

        UTF8Encoding utf8 = new UTF8Encoding();
        Byte[] key = utf8.GetBytes(apiKey);
        Byte[] message = utf8.GetBytes(data);

        HMACSHA256 hash = new HMACSHA256(key);
        var signature = hash.ComputeHash(message);
        return Convert.ToBase64String(signature);
    }
}