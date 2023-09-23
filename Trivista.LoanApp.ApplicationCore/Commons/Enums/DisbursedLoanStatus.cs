namespace Trivista.LoanApp.ApplicationCore.Commons.Enums;

public enum DisbursedLoanStatus
{
    Delinquent = 0,
    Disbursed = 1,
    CollectedRepayment = 2,
    DuePayment = 3,
    WrittenOff = 4,
    DefaultingLoan = 5,
    MaturedLoan = 6,
    None = 7
}