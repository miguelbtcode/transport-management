using Identity.Modules.Dtos;

namespace Identity.Modules.Features.GetModules;

public record GetModulesResponse(List<ModuleDto> Modules);

public class GetModulesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/identity/modules",
                async (ISender sender) =>
                {
                    var result = await sender.SendAsync(new GetModulesQuery());
                    var response = result.Adapt<GetModulesResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Modules")
            .WithName("GetModules")
            .Produces<GetModulesResponse>(StatusCodes.Status200OK)
            .WithSummary("Get Modules")
            .WithDescription("Get list of available modules")
            .RequireAdmin();
    }
}
