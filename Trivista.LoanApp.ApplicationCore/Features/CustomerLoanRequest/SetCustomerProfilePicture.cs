using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class SetCustomerProfilePictureController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/Customer/ProfilePicture", HandleCustomerProfilePicture)
            .WithName("Profile Picture")
            .RequireAuthorization()
            .WithTags("Customer");
    }
    
    private async Task<IResult> HandleCustomerProfilePicture(IMediator mediator, [FromBody]SetCustomerProfilePictureCommand model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x => x);
    }
}

public sealed record ProfilePictureDto
{
    public string ProfilePictureFileName { get; set; }
    
    public string ProfilePictureFileType { get; set; }
    
    public long ProfilePictureFileLength { get; set; }
    
    public string ProfilePictureFile { get; set; }

    public static explicit operator ProfilePicture(ProfilePictureDto profilePictureDto)
    {
        return new ProfilePicture()
        {
            ProfilePictureFileName = profilePictureDto.ProfilePictureFileName, //Required
            ProfilePictureFileType = profilePictureDto.ProfilePictureFileType,
            ProfilePictureFileLength = profilePictureDto.ProfilePictureFileLength, //Required
            ProfilePictureFile = profilePictureDto.ProfilePictureFile, //Required
        };
    }
}

public class SetCustomerProfilePictureCommandValidation : AbstractValidator<SetCustomerProfilePictureCommand>
{
    public SetCustomerProfilePictureCommandValidation()
    {
        //Salary details validation
        RuleFor(x => x.profilePictureDto.ProfilePictureFileName).NotEqual("string")
                                    .NotEmpty().NotNull().WithMessage("Profile picture name must be set");
        RuleFor(x => x.profilePictureDto.ProfilePictureFileLength).GreaterThan(0).WithMessage("Profile picture must be set");
        RuleFor(x => x.profilePictureDto.ProfilePictureFileType).NotEqual("string").NotNull().NotEmpty().WithMessage("Profile picture must be set");
        RuleFor(x => x.profilePictureDto.ProfilePictureFile).NotNull().NotEmpty().WithMessage("Profile picture must be set");
    }
}

public sealed record SetCustomerProfilePictureCommand(Guid CustomerId, ProfilePictureDto profilePictureDto) : IRequest<Result<Unit>>;

public sealed class SetCustomerProfilePictureCommandHandler : IRequestHandler<SetCustomerProfilePictureCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly ILogger<SetCustomerProfilePictureCommandHandler> _logger;

    public SetCustomerProfilePictureCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<SetCustomerProfilePictureCommandHandler> logger)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(SetCustomerProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var validator = new SetCustomerProfilePictureCommandValidation();
        var exceptionResult = await TrivistaValidationException<SetCustomerProfilePictureCommandValidation, SetCustomerProfilePictureCommand>
            .ManageException<Unit>(validator, request, cancellationToken, Unit.Value);
        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        var customer = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        if (customer == null)
            return new Result<Unit>(ExceptionManager.Manage("Customer", "Customer does not exist"));
        
        var profilePicture = (ProfilePicture)request.profilePictureDto;
            
        customer.SetProfilePicture(profilePicture);

        _trivistaDbContext.Customer.Update(customer);

        var savedLoanRequestResponse = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (savedLoanRequestResponse > 0)
        {
            return Unit.Value;
        }
            
        return  new Result<Unit>(ExceptionManager.Manage("Customer", "Unable to save customer profile picture"));
    }
}