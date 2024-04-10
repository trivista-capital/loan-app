namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record SalaryDetails
{
    public decimal AverageMonthlyNetSalary { get; set; }  = default!;
    public string SalaryAccountNumber { get; set; } = default!;
    public string BankName { get; set; } = default!;
    public string AccountName { get; set; } = default!;
    
    public string BankCode { get; set; } = default!;
}