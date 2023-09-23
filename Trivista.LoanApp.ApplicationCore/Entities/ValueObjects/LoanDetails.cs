using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record LoanDetails
{
    public decimal LoanAmount { get; set; }
    public decimal LoanBalance { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    public RepaymentScheduleType RepaymentScheduleType { get; set; }
}