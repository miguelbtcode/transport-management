namespace Identity.Modules.Features.DeleteModule;

public record DeleteModuleResponse(bool IsSuccess);

public class DeleteModuleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/identity/modules/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.SendAsync(new DeleteModuleCommand(id));
                    var response = result.Value.Adapt<DeleteModuleResponse>();
                    return Results.Ok(response);
                }
            )
            .WithTags("Modules")
            .WithName("DeleteModule")
            .Produces<DeleteModuleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Delete Module")
            .WithDescription("Delete (deactivate) a module from the system")
            .RequireAdmin();
    }
}
