using Carter;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApprovalConfiguration;

public sealed class GetApprovalWorkflowConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
         app.MapGet("/getApprovalConfiguration", HandleGetLoanApprovalConfiguration)
                    .WithName("Get-Loan-Approval")
                    .WithTags("Loan Approval Configuration");
    }
    
    /// <summary>
    /// Says howdy to the name
    /// </summary>
    /// <remarks>Awesomeness!</remarks>
    /// <param name="changePasswordModel" example="Khalid">name</param>
    /// <param name="mediator">name</param>
    /// <response code="200">Howdy</response>
    private static async Task<IResult> HandleGetLoanApprovalConfiguration(IMediator mediator)
    {
        var result = await mediator.Send(new GetApprovalWorkflowConfigurationCommand());
        return result.ToOk(x=>x);
    }
}

public sealed class GetApprovalWorkflowConfiguration
{
    public Guid Id { get; set; }
    public string Action { get; set; }
    
    public List<GetApprovalWorkflowApplicationRoleConfigurationDto> Roles { get; set; }
        = new List<GetApprovalWorkflowApplicationRoleConfigurationDto>();

    public static explicit operator GetApprovalWorkflowConfiguration(ApprovalWorkflowConfiguration dto)
    {
        return new GetApprovalWorkflowConfiguration()
        {
            Id = dto.Id,
            Action = dto.Action
        };
    }
}

public sealed class GetApprovalWorkflowApplicationRoleConfigurationDto
{
    public int Id { get; set; }
    
    public Guid RoleId { get; set; }
    
    public bool CanOverrideApproval { get; set; }
    
    public int Hierarchy { get; set; }
    
    public string RoleName { get; set; }
    
    public static explicit operator GetApprovalWorkflowApplicationRoleConfigurationDto(ApprovalWorkflowApplicationRoleConfiguration dto)
    {
        return new GetApprovalWorkflowApplicationRoleConfigurationDto
            {
                Id = dto.Id,
                CanOverrideApproval = dto.CanOverrideAllApprovals,
                RoleId = dto.RoleId,
                RoleName = dto.RoleName,
                Hierarchy = dto.Hierarchy
            };
    }
}

public sealed record GetApprovalWorkflowConfigurationCommand: IRequest<Result<GetApprovalWorkflowConfiguration>>;

public sealed class GetApprovalWorkflowConfigurationCommandHandler : IRequestHandler<GetApprovalWorkflowConfigurationCommand, Result<GetApprovalWorkflowConfiguration>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public GetApprovalWorkflowConfigurationCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<GetApprovalWorkflowConfiguration>> Handle(GetApprovalWorkflowConfigurationCommand request, CancellationToken cancellationToken)
    {
        var workflow = await _trivistaDbContext.ApprovalWorkflowConfiguration
                       .Include(x=>x.Roles)
                       .FirstOrDefaultAsync(cancellationToken);
        
        if(workflow == null)
            return new Result<GetApprovalWorkflowConfiguration>(ExceptionManager.Manage("Approval Workflow", "Work flow does not exist"));
        if(!workflow.Roles.Any())
            return new Result<GetApprovalWorkflowConfiguration>(ExceptionManager.Manage("Approval Workflow", "Work flow configuration roles does not exist"));

        var flow = (GetApprovalWorkflowConfiguration)workflow;
        foreach (var role in workflow.Roles)
        {
            var roleName = await _trivistaDbContext.ApplicationRole.Where(x => x.Id == role.RoleId)
                .Select(x=>x.Name)
                .FirstOrDefaultAsync(cancellationToken);
            role.RoleName = roleName;
            flow.Roles.Add((GetApprovalWorkflowApplicationRoleConfigurationDto)role);
        }

        return flow;
    }
}
