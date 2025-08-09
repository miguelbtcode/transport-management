namespace Identity.Roles.Features.ReactivateRole;

public record ReactivateRoleRequest(Guid RoleId);

public record ReactivateRoleResponse(bool IsSuccess, string Message);

public class ReactivateRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/identity/roles/{roleId:guid}/reactivate",
                async (Guid roleId, ISender sender) =>
                {
                    var command = new ReactivateRoleCommand(roleId);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<ReactivateRoleResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Roles")
            .WithName("ReactivateRole")
            .Produces<ReactivateRoleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Reactivate Role")
            .WithDescription("Reactivate a previously deactivated role")
            .RequireAdmin();
    }
}
