using FluentValidation;
using LanguageExt.Common;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Exceptions;

public sealed class TrivistaValidationException<T, Tt> where T : AbstractValidator<Tt>
{
    public static async Task<Result<TResult>> ManageException<TResult>(T validator, Tt type,  CancellationToken token, TResult result) 
    {
        var validationResult = await validator.ValidateAsync(type, token);
        if (validationResult.IsValid) return result;
        var validationException = new ValidationException(validationResult.Errors);
        return new Result<TResult>(validationException);
    }
}