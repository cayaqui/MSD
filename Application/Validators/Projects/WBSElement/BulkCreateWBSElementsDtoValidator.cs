using FluentValidation;
using Core.DTOs.Projects.WBSElement;
using Domain.Interfaces;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for BulkCreateWBSElementsDto
/// </summary>
public class BulkCreateWBSElementsDtoValidator : AbstractValidator<BulkCreateWBSElementsDto>
{
    public BulkCreateWBSElementsDtoValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Elements)
            .NotEmpty().WithMessage("Debe especificar al menos un elemento para crear")
            .Must(elements => elements.Count <= 100).WithMessage("No se pueden crear más de 100 elementos a la vez");

        RuleForEach(x => x.Elements)
            .ChildRules(element =>
            {
                element.RuleFor(x => x.ProjectId)
                    .NotEmpty().WithMessage("El ID del proyecto es requerido");

                element.RuleFor(x => x.Code)
                    .NotEmpty().WithMessage("El código WBS es requerido")
                    .MaximumLength(50).WithMessage("El código no puede exceder 50 caracteres")
                    .Matches(@"^[0-9]+(\.[0-9]+)*$").WithMessage("El código debe seguir el formato numérico (ej: 1.2.3)");

                element.RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("El nombre es requerido")
                    .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

                element.RuleFor(x => x.Description)
                    .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");

                element.RuleFor(x => x.ElementType)
                    .IsInEnum().WithMessage("El tipo de elemento WBS no es válido");
            });

        RuleFor(x => x.Elements)
            .Must(HaveUniqueCodesWithinBatch).WithMessage("Los códigos WBS deben ser únicos dentro del lote")
            .Must(HaveConsistentProject).WithMessage("Todos los elementos deben pertenecer al mismo proyecto");
    }

    private bool HaveUniqueCodesWithinBatch(List<CreateWBSElementDto> elements)
    {
        return elements.Select(e => e.Code).Distinct().Count() == elements.Count;
    }

    private bool HaveConsistentProject(List<CreateWBSElementDto> elements)
    {
        if (!elements.Any()) return true;
        var firstProjectId = elements.First().ProjectId;
        return elements.All(e => e.ProjectId == firstProjectId);
    }
}