using Identity.Modules.Dtos;
using Identity.Modules.Models;

namespace Identity.Modules.Features.UpdateModule;

public record UpdateModuleCommand(Guid ModuleId, UpdateModuleDto Module)
    : ICommand<Result<UpdateModuleResult>>;

public record UpdateModuleResult(bool IsSuccess);

public class UpdateModuleCommandValidator : AbstractValidator<UpdateModuleCommand>
{
    public UpdateModuleCommandValidator()
    {
        RuleFor(x => x.ModuleId).NotEmpty().WithMessage("ModuleId es requerido");
        RuleFor(x => x.Module.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Module.Description).MaximumLength(255);
    }
}

internal class UpdateModuleHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateModuleCommand, Result<UpdateModuleResult>>
{
    public async Task<Result<UpdateModuleResult>> HandleAsync(
        UpdateModuleCommand command,
        CancellationToken cancellationToken
    )
    {
        var moduleRepository = unitOfWork.Repository<Module>();

        // Verificar que el módulo existe
        var module = await moduleRepository.FirstOrDefaultAsync(
            m => m.Id == command.ModuleId && m.Enabled,
            asNoTracking: false,
            cancellationToken
        );

        if (module == null)
            return ModuleErrors.NotFound(command.ModuleId);

        // Verificar si el nombre ya existe en otro módulo
        var existingModuleWithName = await moduleRepository.FirstOrDefaultAsync(
            m => m.Name.ToLower() == command.Module.Name.ToLower() && m.Id != command.ModuleId,
            asNoTracking: true,
            cancellationToken
        );

        if (existingModuleWithName != null)
            return ModuleErrors.NameAlreadyExists(command.Module.Name);

        // Ejecutar en transacción
        await unitOfWork.ExecuteInTransactionAsync(
            () =>
            {
                module.Update(command.Module.Name, command.Module.Description);
                return Task.CompletedTask;
            },
            cancellationToken
        );

        return new UpdateModuleResult(true);
    }
}
