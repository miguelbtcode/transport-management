namespace Identity.Users.Features.ReactivateUser;

public record ReactivateUserRequest(Guid UserId);

public record ReactivateUserResponse(bool IsSuccess, string Message);

public class ReactivateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/identity/users/{userId:guid}/reactivate",
                async (Guid userId, ISender sender) =>
                {
                    var command = new ReactivateUserCommand(userId);
                    var result = await sender.SendAsync(command);
                    var response = result.Value.Adapt<ReactivateUserResponse>();

                    return Results.Ok(response);
                }
            )
            .WithTags("Users")
            .WithName("ReactivateUser")
            .Produces<ReactivateUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Reactivate User")
            .WithDescription("Reactivate a previously deactivated user")
            .RequireAdmin();
    }
}
