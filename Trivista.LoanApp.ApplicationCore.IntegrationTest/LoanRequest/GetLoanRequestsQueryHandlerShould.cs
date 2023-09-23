using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Model;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;
using Trivista.LoanApp.ApplicationCore.IntegrationTest.Setup;

namespace Trivista.LoanApp.ApplicationCore.IntegrationTest.LoanRequest;

[Collection("Shared Test Collection")]
public class GetLoanRequestsQueryHandlerShould
{
    private readonly GetLoanRequestsQueryHandler _getLoanRequestsQuery;
    public GetLoanRequestsQueryHandlerShould(BaseTestSetup fixture)
    {
        var dbContext = fixture.Services.GetRequiredService<TrivistaDbContext>();
        var tokenManager = fixture.Services.GetRequiredService<TokenManager>();
        _getLoanRequestsQuery = new GetLoanRequestsQueryHandler(dbContext, tokenManager);
    }
    
    [Fact]
    private async Task Return_True_Given_All_Parameters_Are_Valid()
    {
        //Arrange
        var request = new GetLoanRequestsQuery(Guid.Empty, LoanApplicationStatus.Active, "", 1, 1, 10);
        
        //Act
        var response = await _getLoanRequestsQuery.Handle(request, new CancellationToken());

        //Assert
        response.IsSuccess.ShouldBeTrue();
    }
}