using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public class PayStackService: IPayStackService
{
    private readonly HttpClient _client;

    private readonly ILogger<PayStackService> _logger;

    public PayStackService(HttpClient client, ILogger<PayStackService> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task<BanksDto> GetBanks()
    {
        try
        {
            var httpResult = await _client.GetAsync($"bank");
            var result = await httpResult.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<BanksDto>(result);
            if (httpResult.IsSuccessStatusCode)
            {
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", deserializedResponse);
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }
    
    public async Task<ResolveAccountDto> ResolveAccount(string accountNumber, string bankCode)
    {
        try
        {
            var httpResult = await _client.GetAsync($"bank/resolve?account_number={accountNumber}&bank_code={bankCode}");
            var result = await httpResult.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResolveAccountDto>(result);
            if (httpResult.IsSuccessStatusCode)
            {
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", deserializedResponse);
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }
    
    public async Task<TransferRecipientResponseDto> TransferRecipient(TransferRecipientRequestDto dto)
    {
        try
        {
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResult = await _client.PostAsync("transferrecipient", body);
            var result = await httpResult.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<TransferRecipientResponseDto>(result);
            if (httpResult.IsSuccessStatusCode)
            {
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", result);
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return null;
    }
    
    public async Task<TransferResponseDto> Transfer(TransferRequestDto dto)
    {
        try
        {
            dto.Amount = (Convert.ToInt32(dto.Amount) * 100);
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResult = await _client.PostAsync("transfer", body);
            var result = await httpResult.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<TransferResponseDto>(result);
            if (httpResult.IsSuccessStatusCode)
            {
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", deserializedResponse);
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
        }

        return null;
    }
    
    public async Task<FinalTransferResponseDto> FinalizeTransfer(FinalTransferRequestDto dto)
    {
        try
        {
            var json = JsonConvert.SerializeObject(dto);
            var body = new StringContent(json, Encoding.UTF8, "application/json");
            var httpResult = await _client.PostAsync("transfer/finalize_transfer", body);
            var result = await httpResult.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<FinalTransferResponseDto>(result);
            if (httpResult.IsSuccessStatusCode)
            {
                _logger.LogInformation("Creation of Customer was pushed successfully wth response : {Response}", deserializedResponse);
                return deserializedResponse;
            }
            _logger.LogInformation("Publishing of customer was not successful with message: {Message}", deserializedResponse);
            return deserializedResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            
        }

        return null;
    }
}