using Identity.Permissions.Dtos;

namespace Identity.Users.Features.GetUserPermissions;

public record GetUserPermissionsResponse(List<PermisoDto> Permissions);

public class GetUserPermissionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/identity/users/{userId:guid}/permissions",
                async (Guid userId, ISender sender) =>
                {
                    var result = await sender.SendAsync(new GetUserPermissionsQuery(userId));
                    var response = result.Adapt<GetUserPermissionsResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Users")
            .WithName("GetUserPermissions")
            .Produces<GetUserPermissionsResponse>(StatusCodes.Status200OK)
            .WithSummary("Get User Permissions")
            .WithDescription("Get all permissions for a specific user")
            .RequireAdmin();
    }
}
