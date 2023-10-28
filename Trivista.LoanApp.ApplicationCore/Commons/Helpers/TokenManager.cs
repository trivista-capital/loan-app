using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Trivista.LoanApp.ApplicationCore.Commons.Helpers;

public class TokenManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenManager(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string GetRoleName()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User;
        var roleName = currentUser?.Claims?.FirstOrDefault(x => x.Type == "RoleName").Value;
        return roleName!;
    }

    public string GetRoleId()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User;
        if (!currentUser.Claims.Any())
            return string.Empty;
        var roleId = currentUser?.Claims?.FirstOrDefault(x => x.Type == "RoleId").Value;
        return roleId!;
    }
    
    public string GetUserId()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User;
        var roleId = currentUser?.Claims?.FirstOrDefault(x => x.Type == "sub").Value;
        return roleId!;
    }

    public string GetEmail()
    {
        var currentUser = _httpContextAccessor?.HttpContext?.User;
        var roleId = currentUser?.Claims?.FirstOrDefault(x => x.Type == "email").Value;
        return roleId!;
    }
}