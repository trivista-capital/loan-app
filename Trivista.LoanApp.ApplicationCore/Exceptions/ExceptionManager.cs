using FluentValidation;
using FluentValidation.Results;

namespace Trivista.LoanApp.ApplicationCore.Exceptions;

public class ExceptionManager
{
    public static ValidationException Manage(string key, string message)
    {
        return new ValidationException(new List<ValidationFailure>()
        {
            new ValidationFailure(key, message)
        });
    }
}