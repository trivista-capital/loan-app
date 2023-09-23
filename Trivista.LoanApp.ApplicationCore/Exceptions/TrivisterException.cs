namespace Trivista.LoanApp.ApplicationCore.Exceptions;

public class TrivistaException: Exception
{
    public int _errorCode;

    public readonly string _errorMessage = string.Empty;
    
    public TrivistaException(): base() { }

    public TrivistaException(string message, int errorCode) : base(message)
    {
        this._errorCode = errorCode;
        this._errorMessage = message;
    }

    public TrivistaException(string message, Exception ex) : base(message, ex) { }
}