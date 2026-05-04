using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Specifications;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class GeCustomerProfilePictureController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customer/profilepictire/{id}", HandleProfilePictureRequest)
            .WithName("Customer Profile Picture")
            .WithTags("Customer")
            .RequireAuthorization()
            .RequireCors("AllowSpecificOrigins");
    }
    
    private async Task<IResult> HandleProfilePictureRequest(IMediator mediator, [FromQuery]Guid id)
    {
        var result = await mediator.Send(new ProfilePictureRequestQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record GetProfilePictureRequestDto
{
    public Guid Id { get; set; }
    
    public string ProfilePictureFileName { get; set; }
    
    public string ProfilePictureFileType { get; set; }
    
    public long ProfilePictureFileLength { get; set; }
    
    public string ProfilePictureFile { get; set; }

    public static explicit operator GetProfilePictureRequestDto(Entities.Customer customer)
    {
        return new GetProfilePictureRequestDto()
        {
            Id = customer.Id,
            ProfilePictureFileName = customer.ProfilePicture.ProfilePictureFileName, //Required
            ProfilePictureFileType = customer.ProfilePicture.ProfilePictureFileType,
            ProfilePictureFileLength = customer.ProfilePicture.ProfilePictureFileLength, //Required
            ProfilePictureFile = customer.ProfilePicture.ProfilePictureFile, //Required
        };
    }
}

public class ProfilePictureRequestQueryValidation : AbstractValidator<ProfilePictureRequestQuery>
{
    public ProfilePictureRequestQueryValidation()
    {
        //LoanRequest Id validation
        RuleFor(x => x.Id).NotEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"))
            .NotNull().NotEmpty().WithMessage("Customer Id is invalid");
    }
}
public sealed record ProfilePictureRequestQuery(Guid Id): IRequest<Result<GetProfilePictureRequestDto>>;

public sealed class ProfilePictureRequestQueryHandler : IRequestHandler<ProfilePictureRequestQuery, Result<GetProfilePictureRequestDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public ProfilePictureRequestQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<GetProfilePictureRequestDto>> Handle(ProfilePictureRequestQuery request, CancellationToken cancellationToken)
    {
        var predicate = CustomerSpecification.WhereId(request.Id);
        var customerFromDb = await _trivistaDbContext.Customer.FirstOrDefaultAsync(predicate, cancellationToken);
        if(customerFromDb == null || customerFromDb.ProfilePicture == null)
            return new Result<GetProfilePictureRequestDto>(ExceptionManager.Manage("Customer", "Customer has not set profile picture"));
        var loanRequest = (GetProfilePictureRequestDto)customerFromDb;
        return loanRequest;
    }
}