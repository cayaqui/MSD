using FluentValidation;
using Core.DTOs.Projects.WBSElement;
using Domain.Interfaces;
using Domain.Entities.WBS;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for CreateWBSElementDto
/// </summary>
public class CreateWBSElementDtoValidator : AbstractValidator<CreateWBSElementDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateWBSElementDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("El ID del proyecto es requerido");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código WBS es requerido")
            .MaximumLength(50).WithMessage("El código no puede exceder 50 caracteres")
            .Matches(@"^[0-9]+(\.[0-9]+)*$").WithMessage("El código debe seguir el formato numérico (ej: 1.2.3)")
            .MustAsync(BeUniqueCode).WithMessage("El código WBS ya existe en el proyecto");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.ElementType)
            .IsInEnum().WithMessage("El tipo de elemento WBS no es válido");

        RuleFor(x => x.ParentId)
            .MustAsync(BeValidParent).WithMessage("El elemento padre especificado no existe")
            .When(x => x.ParentId.HasValue);

        RuleFor(x => x)
            .MustAsync(HaveValidParentType).WithMessage("El elemento padre no puede ser un paquete de trabajo o paquete de planificación")
            .When(x => x.ParentId.HasValue);
    }

    private async Task<bool> BeUniqueCode(CreateWBSElementDto dto, string code, CancellationToken cancellationToken)
    {
        return !await _unitOfWork.Repository<Domain.Entities.WBS.WBSElement>()
            .Query()
            .AnyAsync(w => w.ProjectId == dto.ProjectId && w.Code == code, cancellationToken);
    }

    private async Task<bool> BeValidParent(Guid? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        return await _unitOfWork.Repository<Domain.Entities.WBS.WBSElement>()
            .Query()
            .AnyAsync(w => w.Id == parentId.Value, cancellationToken);
    }

    private async Task<bool> HaveValidParentType(CreateWBSElementDto dto, CancellationToken cancellationToken)
    {
        if (!dto.ParentId.HasValue) return true;

        var parent = await _unitOfWork.Repository<Domain.Entities.WBS.WBSElement>()
            .Query()
            .FirstOrDefaultAsync(w => w.Id == dto.ParentId.Value, cancellationToken);

        return parent != null && parent.CanHaveChildren();
    }
}