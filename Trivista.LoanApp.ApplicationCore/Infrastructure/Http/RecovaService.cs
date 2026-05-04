using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Features.Dto;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http
{
    public interface IRecovaService
    {
        Task<RecovaResponse> CreateConsent(RecovaRequest input); 
    }

    public class RecovaService : IRecovaService
    {
        private readonly HttpClient _client;

        private readonly RecovaOption _recovaOption;

        private readonly ILogger<RecovaService> _logger;

        public RecovaService(HttpClient client, IOptions<RecovaOption> recovaOption, ILogger<RecovaService> logger)
        {
            _client = client;
            _recovaOption = recovaOption.Value;
            _logger = logger;
        }
        public async Task<RecovaResponse> CreateConsent(RecovaRequest request)
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(request);
                var body = new StringContent(serialized, Encoding.UTF8, "application/json");
                var httpResult = await _client.PostAsync("CreateConsentRequest", body);
                var result = await httpResult.Content.ReadAsStringAsync();
                var deserializedResponse = JsonConvert.DeserializeObject<RecovaResponse>(result);
                if (httpResult.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Creation of consent for customer with name: {Customer} was successfully with response : {Response}", request.CustomerName, deserializedResponse);
                    return deserializedResponse!;
                }
                _logger.LogInformation("Creation of consent for customer with name: {Customer} was not successful with message: {Message}", request.CustomerName, deserializedResponse);
                return deserializedResponse!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }

            return new RecovaResponse();
        }
    }
}
