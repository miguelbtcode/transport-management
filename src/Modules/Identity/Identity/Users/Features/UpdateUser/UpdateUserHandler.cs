using Shared.Exceptions;

namespace Identity.Users.Features.UpdateUser;

public record UpdateUserCommand(UpdateUserDto UserDto) : ICommand<Result<UpdateUserResult>>;

public record UpdateUserResult(bool IsSuccess);

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserDto.Id).NotEmpty();
        RuleFor(x => x.UserDto.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.UserDto.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.UserDto.RoleIds).NotEmpty();
    }
}

internal class UpdateUserHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserCommand, Result<UpdateUserResult>>
{
    public async Task<Result<UpdateUserResult>> HandleAsync(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var userRepository = unitOfWork.Repository<User>();
            var roleRepository = unitOfWork.Repository<Role>();

            var user = await userRepository
                .Query(u => u.Id == command.UserDto.Id, asNoTracking: false)
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return UserErrors.NotFound(command.UserDto.Id);

            // Verificar si el email ya existe en otro usuario
            var existingUser = await userRepository.FirstOrDefaultAsync(
                u => u.Email == command.UserDto.Email.ToLower() && u.Id != command.UserDto.Id,
                asNoTracking: true,
                cancellationToken
            );

            if (existingUser != null)
                return UserErrors.EmailAlreadyExists(command.UserDto.Email);

            // Verificar que los roles existen
            var existingRoles = await roleRepository.CountAsync(
                r => command.UserDto.RoleIds.Contains(r.Id) && r.Enabled,
                cancellationToken
            );

            if (existingRoles != command.UserDto.RoleIds.Count)
                return RoleErrors.InvalidRoles;

            // Actualizar perfil
            user.UpdateUserInformation(
                command.UserDto.Name,
                command.UserDto.Email,
                command.UserDto.RoleIds
            );

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return new UpdateUserResult(true);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new BusinessException(
                "User.ConcurrencyError",
                "El usuario fue modificado por otro proceso"
            );
        }
    }
}
