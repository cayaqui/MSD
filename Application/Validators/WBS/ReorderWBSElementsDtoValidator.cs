using Core.DTOs.Projects.WBSElement;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for ReorderWBSElementsDto
/// </summary>
public class ReorderWBSElementsDtoValidator : AbstractValidator<ReorderWBSElementsDto>
{
    public ReorderWBSElementsDtoValidator()
    {
        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("El ID del padre es requerido");

        RuleFor(x => x.Elements)
            .NotEmpty().WithMessage("La lista de elementos no puede estar vacía")
            .Must(HaveUniqueIds).WithMessage("Los IDs de elementos deben ser únicos")
            .Must(HaveUniqueSequenceNumbers).WithMessage("Los números de secuencia deben ser únicos");

        RuleForEach(x => x.Elements)
            .SetValidator(new WBSElementOrderDtoValidator());
    }

    private bool HaveUniqueIds(List<WBSElementOrderDto> elements)
    {
        return elements.Select(e => e.Id).Distinct().Count() == elements.Count;
    }

    private bool HaveUniqueSequenceNumbers(List<WBSElementOrderDto> elements)
    {
        return elements.Select(e => e.SequenceNumber).Distinct().Count() == elements.Count;
    }
}
