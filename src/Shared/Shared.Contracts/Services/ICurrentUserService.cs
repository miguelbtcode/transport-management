namespace Shared.Contracts.Services;

public interface ICurrentUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserEmail();
    bool IsAuthenticated();
}
