namespace Identity.Roles.Features.DeleteRole;

public record DeleteRoleResponse(bool IsSuccess);

public class DeleteRoleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/identity/roles/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var command = new DeleteRoleCommand(id);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<DeleteRoleResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Roles")
            .WithName("DeleteRole")
            .Produces<DeleteRoleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Delete Role")
            .WithDescription("Logical delete of a role (deactivate)")
            .RequireAdmin();
    }
}
