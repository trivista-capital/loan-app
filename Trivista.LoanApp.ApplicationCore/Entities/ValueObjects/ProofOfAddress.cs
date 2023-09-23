namespace Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

public sealed record ProofOfAddress
{
    public string ProofOFAddressFileName { get; set; }
    
    public string ProofOFAddressFileType { get; set; }
    
    public long ProofOFAddressFileLength { get; set; }
    
    public string ProofOFAddressFile { get; set; }
}