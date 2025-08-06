using Microsoft.AspNetCore.Http;
using Shared.Contracts.Services;

namespace Shared.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.User?.FindFirst("userid")?.Value;
    }

    public string? GetCurrentUserEmail()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.User?.FindFirst("email")?.Value;
    }

    public bool IsAuthenticated()
    {
        var context = _httpContextAccessor.HttpContext;
        return context?.User?.Identity?.IsAuthenticated == true;
    }
}
