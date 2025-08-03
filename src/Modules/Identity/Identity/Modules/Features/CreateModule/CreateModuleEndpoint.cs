using Identity.Modules.Dtos;

namespace Identity.Modules.Features.CreateModule;

public record CreateModuleRequest(CreateModuleDto Module);

public record CreateModuleResponse(Guid Id);

public class CreateModuleEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/identity/modules",
                async (CreateModuleRequest request, ISender sender) =>
                {
                    var command = new CreateModuleCommand(request.Module);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<CreateModuleResponse>();

                    return Results.Created($"/identity/modules/{response.Id}", response);
                }
            )
            .WithTags("Modules")
            .WithName("CreateModule")
            .Produces<CreateModuleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Create Module")
            .WithDescription("Create a new module in the system")
            .RequireAdmin();
    }
}
