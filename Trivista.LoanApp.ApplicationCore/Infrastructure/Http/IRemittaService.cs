namespace Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

public interface IRemittaService
{
    Task<GetSalaryHistoryResponseDto> SalaryHistory(GetSalaryHistoryRequestDto model, string loanRequestId);

    Task<LoanDisbursementResponseDto> DisburseLoan(LoanDisbursementRequestDto model);
}