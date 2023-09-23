namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record kycDetails
{
    public string CustomerFirstName { get; set; }
    public string CustomerMiddleName { get; set; }
    public string CustomerLastName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerAddress { get; set; }
    public string CustomerCity { get; set; }
    public string CustomerState { get; set; }
    public string CustomerCountry { get; set; }
    public string CustomerPostalCode { get; set; }
    public string CustomerOccupation { get; set; }
    public string CustomerPhoneNumber { get; set; }
}