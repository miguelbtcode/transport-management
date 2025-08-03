using Identity.Modules.Dtos;
using Identity.Modules.Models;

namespace Identity.Modules.Features.CreateModule;

public record CreateModuleCommand(CreateModuleDto Module) : ICommand<Result<CreateModuleResult>>;

public record CreateModuleResult(Guid Id);

public class CreateModuleCommandValidator : AbstractValidator<CreateModuleCommand>
{
    public CreateModuleCommandValidator()
    {
        RuleFor(x => x.Module.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Module.Description).MaximumLength(255);
    }
}

internal class CreateModuleHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateModuleCommand, Result<CreateModuleResult>>
{
    public async Task<Result<CreateModuleResult>> HandleAsync(
        CreateModuleCommand command,
        CancellationToken cancellationToken
    )
    {
        var moduleRepository = unitOfWork.Repository<Module>();

        // Verificar si el módulo ya existe
        var existingModule = await moduleRepository.FirstOrDefaultAsync(
            m => m.Name.ToLower() == command.Module.Name.ToLower(),
            asNoTracking: true,
            cancellationToken
        );

        if (existingModule != null)
            return ModuleErrors.NameAlreadyExists(command.Module.Name);

        // Ejecutar en transacción
        var moduleId = await unitOfWork.ExecuteInTransactionAsync(
            async () =>
            {
                var module = Module.Create(
                    command.Module.Name,
                    command.Module.Description,
                    command.Module.Enabled
                );
                await moduleRepository.AddAsync(module, cancellationToken);
                return module.Id;
            },
            cancellationToken
        );

        return new CreateModuleResult(moduleId);
    }
}
