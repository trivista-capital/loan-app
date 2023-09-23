using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.BankCode;

public class GetBankCodeController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getBanks", GetBanksHandler)
            .WithName("Bank Names")
            .WithTags("Admin");
    }

    private async Task<IResult> GetBanksHandler(IMediator mediator)
    {
        var result = await mediator.Send(new GetBanksQuery());
        return result.ToOk(x => x);
    }
}

public sealed record GetBanksQuery() : IRequest<Result<SelectActiveRequestBanksJSONObjectResponseDto>>;

public sealed class GetBanksQueryHandler : IRequestHandler<GetBanksQuery, Result<SelectActiveRequestBanksJSONObjectResponseDto>>
{
    private readonly IMbsService _mbsService;

    public GetBanksQueryHandler(IMbsService mbsService)
    {
        _mbsService = mbsService;
    }
    public async Task<Result<SelectActiveRequestBanksJSONObjectResponseDto>> Handle(GetBanksQuery request, CancellationToken cancellationToken)
    {
        var result = await _mbsService.SelectActiveRequestBanks();
        return result;
    }
}
