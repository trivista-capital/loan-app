using Trivista.LoanApp.ApplicationCore.Features.Integrations;

namespace Trivista.LoanApp.ApplicationCore.Interfaces;

public interface ISmileIdService
{
    Task<CustomerPersonalDetailsResponseDto> VerifyUserDetails(GetCustomerPersonalDetailsRequestDto request, string timeStamp, Guid paramsId);
}