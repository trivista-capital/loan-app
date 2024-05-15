using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Role;

public sealed class GetRoles: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/roles", GetRolesHandler)
            .WithName("Get Role")
            .RequireAuthorization()
            .WithTags("Role");
    }

    private static async Task<IResult> GetRolesHandler(IMediator mediator)
    {
        var result = await mediator.Send(new GetRoleQuery());
        return result.ToOk(x => x);
    }
}

public sealed record GetRolesDto
{
    public Guid Id { get; set; }
    
    public string RoleName { get; set; }
    
    public string Description { get; set; }

    public static implicit operator GetRolesDto(ApplicationRole role)
    {
        return new GetRolesDto()
        {
            Id = role.Id,
            RoleName = role.Name,
            Description = role.Description
        };
    }
}

public sealed record GetRoleQuery: IRequest<Result<List<GetRolesDto>>>;

public sealed record GetRoleQueryHandler : IRequestHandler<GetRoleQuery, Result<List<GetRolesDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetRoleQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<List<GetRolesDto>>> Handle(GetRoleQuery request, CancellationToken cancellationToken)
    {
        List<GetRolesDto> roleList = new List<GetRolesDto>();
        var roles = await _trivistaDbContext.ApplicationRole.Select(x=>x).ToListAsync(cancellationToken);
        // foreach (var role in roles)
        // {
        //     roleList.Add((GetRolesDto)role); 
        // }
        // return roleList;
        return roles.Select(role => (GetRolesDto)role).ToList();
    }
}