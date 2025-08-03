using Identity.Permissions.Dtos;

namespace Identity.Permissions.Features.GetPermissions;

public record GetPermissionsResponse(List<PermissionDetailDto> Permissions);

public class GetPermissionsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/identity/permissions",
                async (
                    Guid? roleId,
                    Guid? moduleId,
                    Guid? permissionTypeId,
                    bool? enabled,
                    ISender sender
                ) =>
                {
                    var result = await sender.SendAsync(
                        new GetPermissionsQuery(roleId, moduleId, permissionTypeId, enabled)
                    );
                    var response = result.Adapt<GetPermissionsResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Permissions")
            .WithName("GetPermissions")
            .Produces<GetPermissionsResponse>(StatusCodes.Status200OK)
            .WithSummary("Get Permissions")
            .WithDescription(
                "Get permissions with optional filters by role, module, permission type, and enabled status"
            )
            .RequireAdmin();
    }
}
