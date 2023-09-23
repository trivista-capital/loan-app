namespace Trivista.LoanApp.ApplicationCore.Services.Payment;

public interface IPayStackService
{
    Task<ResolveAccountDto> ResolveAccount(string accountNumber, string bankCode);

    Task<TransferRecipientResponseDto> TransferRecipient(TransferRecipientRequestDto dto);

    Task<TransferResponseDto> Transfer(TransferRequestDto dto);

    Task<FinalTransferResponseDto> FinalizeTransfer(FinalTransferRequestDto dto);

    Task<BanksDto> GetBanks();
}