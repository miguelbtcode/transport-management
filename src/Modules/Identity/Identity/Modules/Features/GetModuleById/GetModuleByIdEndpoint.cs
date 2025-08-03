using Identity.Modules.Dtos;

namespace Identity.Modules.Features.GetModuleById;

public record GetModuleByIdResponse(ModuleDto Module);

public class GetModuleByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/identity/modules/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var result = await sender.SendAsync(new GetModuleByIdQuery(id));
                    var response = result.Adapt<GetModuleByIdResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Modules")
            .WithName("GetModuleById")
            .Produces<GetModuleByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Module By Id")
            .WithDescription("Get a specific module by its ID")
            .RequireAdmin();
    }
}
