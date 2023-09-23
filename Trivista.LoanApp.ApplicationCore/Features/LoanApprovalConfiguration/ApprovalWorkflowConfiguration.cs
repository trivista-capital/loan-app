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

public sealed class ApprovalWorkflowConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
         app.MapPost("/configureLoanApproval", HandleLoanApprovalConfiguration)
                    .WithName("Configure-Loan-Approval")
                    .WithTags("Loan Approval Configuration");
    }
    
    /// <summary>
    /// Says howdy to the name
    /// </summary>
    /// <remarks>Awesomeness!</remarks>
    /// <param name="changePasswordModel" example="Khalid">name</param>
    /// <param name="mediator">name</param>
    /// <response code="200">Howdy</response>
    private async Task<IResult> HandleLoanApprovalConfiguration(IMediator mediator, [FromBody]ApprovalWorkflowConfigurationCommand model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x=>x);
    }
}

public sealed class ApprovalWorkflowApplicationRoleConfigurationDto
{
    public Guid RoleId { get; set; }
    public bool CanOverrideApproval { get; set; }
    public int Hierarchy { get; set; }

    public static List<ApprovalWorkflowApplicationRoleConfiguration> ToApprovalWorkflowApplicationRoleConfiguration(IEnumerable<ApprovalWorkflowApplicationRoleConfigurationDto> dto)
    {
        return dto.Select(x => 
            new ApprovalWorkflowApplicationRoleConfiguration(x.RoleId, x.CanOverrideApproval, x.Hierarchy)
       ).ToList();
    }
}


public sealed record ApprovalWorkflowConfigurationCommand(ApprovalTypes Action,
    List<ApprovalWorkflowApplicationRoleConfigurationDto> ApprovalWorkflowApplicationRoleConfigurationDto) : IRequest<Result<Unit>>;

public sealed class ApprovalWorkflowConfigurationCommandHandler : IRequestHandler<ApprovalWorkflowConfigurationCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public ApprovalWorkflowConfigurationCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<Unit>> Handle(ApprovalWorkflowConfigurationCommand request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(request.Action.ToString()))
            throw new TrivistaException("Approval workflow action name can not be null", 400);
        if (!request.ApprovalWorkflowApplicationRoleConfigurationDto.Any())
            return new Result<Unit>(new ValidationException(new List<ValidationFailure>() {
                new ValidationFailure("Approval Workflow", "Approval workflow roles can not be null")
            }));
        var workflow = await _trivistaDbContext.ApprovalWorkflowConfiguration.AnyAsync(cancellationToken);
        if(workflow)
            return new Result<Unit>(ExceptionManager.Manage("Approval Workflow", "Approval Work flow already exist"));
        var workflowName = EnumHelpers.Convert(request.Action);
        var doesWorkflowExist =
            await _trivistaDbContext.ApprovalWorkflowConfiguration.AnyAsync(x => x.Action == workflowName,
                cancellationToken);
        if(doesWorkflowExist)
            return new Result<Unit>(new ValidationException(new List<ValidationFailure>() {
                new ValidationFailure("Approval Workflow", "Approval workflow already exists")
            }));
        var approvalWorkflowConfiguration = ApprovalWorkflowConfiguration.Factory.Build(EnumHelpers.Convert(request.Action),
            ApprovalWorkflowApplicationRoleConfigurationDto.ToApprovalWorkflowApplicationRoleConfiguration(
                request.ApprovalWorkflowApplicationRoleConfigurationDto));
        _trivistaDbContext.ApprovalWorkflowConfiguration.Add(approvalWorkflowConfiguration);
        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
            return Unit.Value;
        return new Result<Unit>(new ValidationException(new List<ValidationFailure>() {
            new ValidationFailure("Approval Workflow", "Unable to create Approval workflow")
        }));
    }
}
