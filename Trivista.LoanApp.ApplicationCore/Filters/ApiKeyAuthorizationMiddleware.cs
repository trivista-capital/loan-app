using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivista.LoanApp.ApplicationCore.Filters
{
    public class ApiKeyAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";
        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthorizationMiddleware(RequestDelegate next, IApiKeyValidator apiKeyValidator)
        {
            _next = next;
            _apiKeyValidator = apiKeyValidator;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKey) ||
            //    !_apiKeyValidator.IsValid(apiKey!))
            //{
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    await context.Response.WriteAsync("Unauthorized");
            //    return;
            //}

            await _next(context);
        }
    }
}
