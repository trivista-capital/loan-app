using Newtonsoft.Json;

namespace Trivista.LoanApp.ApplicationCore.Commons.Options;

public class SmileIdOption
{
    public string SandBoxApiKey { get; set; }
    public string Url { get; set; }
    public string Country { get; set; }
    public string IdType { get; set; }
    
    public string Signature { get; set; }
    public string PartnerId { get; set; }
    
    public string Source_Sdk { get; set; }
    
    public string Source_Sdk_Version { get; set; }
    
    public string First_Name { get; set; }
    
    public string Last_Name { get; set; }
    
    public string Call_Back_Url { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Job_Type { get; set; }
}