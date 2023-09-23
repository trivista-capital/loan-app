using System.Text;
using Newtonsoft.Json;
using Shouldly;
using Trivista.LoanApp.ApplicationCore.Commons.Model;
using Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;
using Trivista.LoanApp.ApplicationCore.IntegrationTest.Setup;

namespace Trivista.LoanApp.ApplicationCore.IntegrationTest.LoanRequest;

[Collection("Shared Test Collection")]
public class LoanRequestControllerShould
{
    private readonly HttpClient _httpClient;
    public LoanRequestControllerShould(BaseTestSetup fixture)
    {
        _httpClient = fixture.HttpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7104");
    }
    
    [Fact]
    private async Task Return_True_Given_All_Parameters_Are_Valid()
    {
        //Arrange
        var cmd = new RequestLoanCommand(Guid.Empty, "", new kycDetailsDto(), new LoanDetailsDto(), new SalaryDetailsDto(), new ProofOfAddressDto(), false);
        var serializedModel = JsonConvert.SerializeObject(cmd);
        var content = new StringContent(serializedModel, Encoding.UTF8, "application/json");
        //Act
        var httpResponse = await _httpClient.PostAsync("/requestLoan", content);
        var httpStringContent = await httpResponse.Content.ReadAsStringAsync();
        var deserializeObject = JsonConvert.DeserializeObject<ResponseModel<bool>>(httpStringContent);
        //Assert
        httpResponse.IsSuccessStatusCode.ShouldBeFalse();
        httpResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest, "true");
        deserializeObject.Value.ShouldBeFalse();
    }
}