
using FluentValidation;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Trivista.LoanApp.ApplicationCore.Commons.Model;
using Trivista.LoanApp.ApplicationCore.Exceptions;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class ControllerExtensions
{
    public static IResult ToOk<TResult, TContract>(this Result<TResult> result, Func<TResult, TContract> mapper)
    {
        return result.Match<IResult>(obj =>
        {
            var response = mapper(obj);
            return Results.Ok(new ResponseModel<TContract>()
            {
                Message = "",
                StatusCode = 00,
                Value = response,
                IsSuccessful = true
            });
        }, exception =>
        {
            switch (exception)
            {
                case TrivistaException { _errorCode : 404 } trivistaException:
                    return Results.NotFound(new ResponseModel<bool>()
                    {
                        ErrorMessage = trivistaException.Message,
                        IsSuccessful = false,
                        StatusCode = trivistaException._errorCode,
                    });
                case TrivistaException { _errorCode : 400 } trivistaException:
                    return Results.BadRequest(new ResponseModel<bool>()
                    {
                        ErrorMessage = trivistaException.Message,
                        IsSuccessful = false,
                        StatusCode = trivistaException._errorCode,
                    });
                case ValidationException validationException:
                {
                    var validationErrors = validationException.Errors.Select(x => x.ErrorMessage).ToArray();
                    var errors = string.Join(", ", validationErrors);
                    return Results.BadRequest(new ResponseModel<bool>()
                    {
                        ErrorMessage = errors,
                        IsSuccessful = false,
                        StatusCode = 400,
                    });
                }
                default:
                    return Results.StatusCode(500);
            }
        });
    }
}
