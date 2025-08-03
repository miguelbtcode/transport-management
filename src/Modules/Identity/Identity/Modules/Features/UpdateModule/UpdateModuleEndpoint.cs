using Identity.Modules.Dtos;

namespace Identity.Modules.Features.UpdateModule;

public record UpdateModuleRequest(UpdateModuleDto Module);

public record UpdateModuleResponse(bool IsSuccess);

public class UpdateModuleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/identity/modules/{id:guid}",
                async (Guid id, UpdateModuleRequest request, ISender sender) =>
                {
                    var command = new UpdateModuleCommand(id, request.Module);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<UpdateModuleResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Modules")
            .WithName("UpdateModule")
            .Produces<UpdateModuleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Module")
            .WithDescription("Update an existing module")
            .RequireAdmin();
    }
}
