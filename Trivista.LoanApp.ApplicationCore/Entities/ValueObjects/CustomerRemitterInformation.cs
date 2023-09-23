using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record CustomerRemitterInformation
{
    public bool IsRemittaUser { get; set; }
    public decimal AverageSixMonthsSalary { get; set; }
    public decimal OtherLoansCollected { get; set; }
    
    //public string Mandate { get; set; }
}