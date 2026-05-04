using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Data.Context;

namespace Trivista.LoanApp.ApplicationCore.Filters
{
    public interface IApiKeyValidator
    {
        Task<bool> IsValid(string apiKey);
    }

    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly TrivistaDbContext _trivistaDbContext;

        public ApiKeyValidator(TrivistaDbContext trivistaDbContext)
        {
            _trivistaDbContext = trivistaDbContext;
        }


        public async Task<bool> IsValid(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                return false;

            var clientApiKey = await _trivistaDbContext.ClientApiKey.FirstOrDefaultAsync(x => x.ApiKey == apiKey);
            if (clientApiKey!.ApiKey!.Equals(apiKey)) {
                return true;
            }
            return false;
        }
    }
}
