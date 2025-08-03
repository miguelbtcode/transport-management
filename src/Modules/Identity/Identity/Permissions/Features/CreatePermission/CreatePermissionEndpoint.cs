using Identity.Permissions.Dtos;

namespace Identity.Permissions.Features.CreatePermission;

public record CreatePermissionRequest(CreatePermissionDto Permission);

public record CreatePermissionResponse(Guid Id);

public class CreatePermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/identity/permissions",
                async (CreatePermissionRequest request, ISender sender) =>
                {
                    var command = new CreatePermissionCommand(request.Permission);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<CreatePermissionResponse>();

                    return Results.Created($"/identity/permissions/{response.Id}", response);
                }
            )
            .WithTags("Permissions")
            .WithName("CreatePermission")
            .Produces<CreatePermissionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Permission")
            .WithDescription("Create a new permission for a role")
            .RequireAdmin();
    }
}
