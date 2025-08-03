using Identity.Permissions.Dtos;

namespace Identity.PermissionTypes.Features.GetPermissionTypes;

public record GetPermissionTypesResponse(List<PermissionTypeDto> PermissionTypes);

public class GetPermissionTypesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/identity/permission-types",
                async (ISender sender) =>
                {
                    var result = await sender.SendAsync(new GetPermissionTypesQuery());
                    var response = result.Adapt<GetPermissionTypesResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("PermissionTypes")
            .WithName("GetPermissionTypes")
            .Produces<GetPermissionTypesResponse>(StatusCodes.Status200OK)
            .WithSummary("Get Permission Types")
            .WithDescription("Get list of available permission types")
            .RequireAdmin();
    }
}
