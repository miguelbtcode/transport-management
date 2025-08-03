using Identity.Permissions.Dtos;

namespace Identity.Permissions.Features.UpdatePermission;

public record UpdatePermissionRequest(UpdatePermissionDto Permission);

public record UpdatePermissionResponse(bool IsSuccess);

public class UpdatePermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/identity/permissions/{id:guid}",
                async (Guid id, UpdatePermissionRequest request, ISender sender) =>
                {
                    var command = new UpdatePermissionCommand(id, request.Permission);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<UpdatePermissionResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Permissions")
            .WithName("UpdatePermission")
            .Produces<UpdatePermissionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Permission")
            .WithDescription("Update an existing permission")
            .RequireAdmin();
    }
}
