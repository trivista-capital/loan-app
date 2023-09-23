namespace Trivista.LoanApp.ApplicationCore.Commons.Model;

public class ResponseModel<T>
{
    public int StatusCode { get; set; }
    public T Value { get; set; }
    public bool IsSuccessful { get; set; }
    public string ErrorMessage { get; set; }
    public string Message { get; set; }
}