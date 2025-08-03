namespace Identity.Permissions.Features.DeletePermission;

public record DeletePermissionResponse(bool IsSuccess);

public class DeletePermissionEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/identity/permissions/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.SendAsync(new DeletePermissionCommand(id));
                    var response = result.Adapt<DeletePermissionResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Permissions")
            .WithName("DeletePermission")
            .Produces<DeletePermissionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Permission")
            .WithDescription("Delete (deactivate) a permission")
            .RequireAdmin();
    }
}
