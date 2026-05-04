using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivista.LoanApp.ApplicationCore.Filters
{
    public class ApiKeyAuthorizationFilter: IAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //string apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

            //if (_apiKeyValidator.IsValid(apiKey))
            //{
            //    context.Result = new UnauthorizedResult();
            //}
        }
    }
}
