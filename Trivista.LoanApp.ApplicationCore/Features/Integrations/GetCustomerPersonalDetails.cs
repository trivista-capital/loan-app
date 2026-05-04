using System.Security.Cryptography;
using System.Text;
using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Interfaces;

namespace Trivista.LoanApp.ApplicationCore.Features.Integrations;

public sealed class GetCustomerPersonalDetailsController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/getCustomerPersonalDetails", GetCustomerPersonalDetailsHandler)
            .WithName("GetCustomerPersonalDetails")
            .RequireAuthorization()
            .WithTags("Name Verification");
    }

    private static async Task<IResult> GetCustomerPersonalDetailsHandler(IMediator mediator,
        [FromBody]GetCustomerPersonalDetailsQuery query)
    {
        var response = await mediator.Send(query);
        return response.ToOk(x => x);
    }
}

public sealed record GetCustomerPersonalDetailsRequestDto
{
    [JsonProperty("country")]
    public string Country { get;set; }
    
    [JsonProperty("id_type")]
    public string IdType { get;set; }
    
    [JsonProperty("id_number")]
    public string IdNumber { get;set; }
    
    [JsonProperty("signature")]
    public string Signature { get;set; }
    
    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonProperty("source_sdk")]
    public string Source_Sdk { get; set; }
    
    [JsonProperty("source_sdk_version")]
    public string Source_Sdk_Version { get; set; }
    
    [JsonProperty("partner_id")]
    public string Partner_id { get; set; }
    
    [JsonProperty("partner_params")]
    public PartnerParams PartnerParams { get; set; }
    
    [JsonProperty("first_name")]
    public string First_Name { get; set; }
    
    [JsonProperty("last_name")]
    public string Last_Name { get; set; }
    
    [JsonProperty("callback_url")]
    public string CallBack_Url { get; set; }
    
    [JsonProperty("phone_number")]
    public string PhoneNumber { get; set; }
}

public sealed class PartnerParams
{
    [JsonProperty("job_id")]
    public string JobId { get; set; }
    
    [JsonProperty("user_id")]
    public string User_Id { get; set; }
    
    [JsonProperty("job type")]
    public string Job_Type { get; set; }
}

public sealed class Actions
{
    public string Verify_ID_Number { get; set; }
    
    public string Return_Personal_Info { get; set; }
}

public sealed record CustomerPersonalDetailsResponseDto(string JSONVersion, string SmileJobID, PartnerParams PartnerParams,
    string ResultType, string ResultText, string ResultCode, string IsFinalResult, Actions Actions, string Country, string IDType, 
    string IDNumber, string ExpirationDate, string FullName, string DOB, string Photo, string signature, string timestamp);

public sealed record GetCustomerPersonalDetailsQuery(string BVN, string FirstName, string LastName, string PhoneNumber): IRequest<Result<CustomerPersonalDetailsResponseDto>>;

public sealed record
    GetCustomerPersonalDetailsQueryHandler : IRequestHandler<GetCustomerPersonalDetailsQuery,
        Result<CustomerPersonalDetailsResponseDto>>
{
    private readonly ISmileIdService _smileIdService;
    public GetCustomerPersonalDetailsQueryHandler(ISmileIdService smileIdService)
    {
        _smileIdService = smileIdService;
    }
    
    public async Task<Result<CustomerPersonalDetailsResponseDto>> Handle(GetCustomerPersonalDetailsQuery request, CancellationToken cancellationToken)
    {
        string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", System.Globalization.CultureInfo.InvariantCulture);
        var paramsId = Guid.NewGuid();
        var response = await _smileIdService.VerifyUserDetails(new GetCustomerPersonalDetailsRequestDto()
        {
            Timestamp = timeStamp,
            IdNumber = request.BVN,
            First_Name = request.FirstName,
            Last_Name = request.LastName,
            PhoneNumber = request.PhoneNumber
        }, timeStamp, paramsId);

        return response;

    }
}