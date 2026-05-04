using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivista.LoanApp.ApplicationCore.Filters
{
    public class ApiKeyAttribute: ServiceFilterAttribute
    {
        public ApiKeyAttribute(): base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
