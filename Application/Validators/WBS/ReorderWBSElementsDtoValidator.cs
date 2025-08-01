using Core.DTOs.WBS;
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
            .NotEmpty().WithMessage("Parent ID is required");

        RuleFor(x => x.Elements)
            .NotEmpty().WithMessage("Elements list cannot be empty")
            .Must(HaveUniqueIds).WithMessage("Element IDs must be unique")
            .Must(HaveUniqueSequenceNumbers).WithMessage("Sequence numbers must be unique");

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
