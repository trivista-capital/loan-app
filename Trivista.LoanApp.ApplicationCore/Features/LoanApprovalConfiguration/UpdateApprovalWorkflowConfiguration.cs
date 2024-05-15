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

public sealed class UpdateApprovalWorkflowConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
         app.MapPost("/loanApprovalConfiguration/update/{id}", HandleUpdateLoanApprovalConfiguration)
                    .WithName("Update-Configure-Loan-Approval")
                    .RequireAuthorization()
                    .WithTags("Loan Approval Configuration");
    }
    
    /// <summary>
    /// Says howdy to the name
    /// </summary>
    /// <remarks>Awesomeness!</remarks>
    /// <param name="changePasswordModel" example="Khalid">name</param>
    /// <param name="mediator">name</param>
    /// <response code="200">Howdy</response>
    private static async Task<IResult> HandleUpdateLoanApprovalConfiguration(IMediator mediator, [FromRoute]Guid id, [FromBody]UpdateApprovalWorkflowConfiguration model)
    {
        var result = await mediator.Send(new UpdateApprovalWorkflowConfigurationCommand(id, model));
        return result.ToOk(x=>x);
    }
}

public sealed class UpdateApprovalWorkflowConfiguration
{
    public Guid Id { get; set; }
    
    public string Action { get; set; }
    
    public List<UpdateApprovalWorkflowApplicationRoleConfigurationDto> Roles { get; set; }
        = new List<UpdateApprovalWorkflowApplicationRoleConfigurationDto>();
}

public sealed class UpdateApprovalWorkflowApplicationRoleConfigurationDto
{
    public int Id { get; set; }
    
    public Guid RoleId { get; set; }
    
    public bool CanOverrideApproval { get; set; }
    
    public int Hierarchy { get; set; }
    
    public static List<ApprovalWorkflowApplicationRoleConfiguration> ToGetApprovalWorkflowApplicationRoleConfigurationDto(IEnumerable<UpdateApprovalWorkflowApplicationRoleConfigurationDto> dto)
    {
        return dto.Select(x => 
            new ApprovalWorkflowApplicationRoleConfiguration(x.RoleId, x.CanOverrideApproval, x.Hierarchy)
        ).ToList();
    }
}

public sealed record UpdateApprovalWorkflowConfigurationCommand(Guid Id, UpdateApprovalWorkflowConfiguration approvalConfiguration) : IRequest<Result<Unit>>;

public sealed class UpdateApprovalWorkflowConfigurationCommandHandler : IRequestHandler<UpdateApprovalWorkflowConfigurationCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public UpdateApprovalWorkflowConfigurationCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<Unit>> Handle(UpdateApprovalWorkflowConfigurationCommand request, CancellationToken cancellationToken)
    {
        if(request.approvalConfiguration == null || !request.approvalConfiguration.Roles.Any())
            return new Result<Unit>(ExceptionManager.Manage("Approval Workflow", "Approval workflow roles can not be null"));
        // var workflowName = EnumHelpers.Convert(request.Action);
        var workflow = await _trivistaDbContext.ApprovalWorkflowConfiguration
                       .Include(x=>x.Roles)
                       .Where(x => x.Id == request.Id)
                       .FirstOrDefaultAsync(cancellationToken);
        
        if(workflow == null)
            return new Result<Unit>(ExceptionManager.Manage("Approval Workflow", "Work flow configuration does not exist"));
        if(!workflow.Roles.Any())
            return new Result<Unit>(ExceptionManager.Manage("Approval Workflow", "Work flow configuration does not exist"));
        foreach (var role in request.approvalConfiguration.Roles)
        {
            var workflowApplicationRoleConfiguration = await _trivistaDbContext.ApprovalWorkflowApplicationRoleConfiguration
                                             .FirstOrDefaultAsync(x => x.Id == role.Id, cancellationToken);
            workflowApplicationRoleConfiguration.SetRoleId(role.RoleId);
            workflowApplicationRoleConfiguration.SetHierarchy(role.Hierarchy);
        }
        await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        var roles = UpdateApprovalWorkflowApplicationRoleConfigurationDto
            .ToGetApprovalWorkflowApplicationRoleConfigurationDto(request.approvalConfiguration.Roles);
        workflow.Roles = roles;
        
        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
            return Unit.Value;
        return new Result<Unit>(new ValidationException(new List<ValidationFailure>() {
            new ValidationFailure("Approval Workflow", "Unable to update Approval workflow")
        }));
    }
}
