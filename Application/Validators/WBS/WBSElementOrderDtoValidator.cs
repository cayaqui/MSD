using Core.DTOs.Projects.WBSElement;
using Core.Enums.Progress;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for WBSElementOrderDto
/// </summary>
public class WBSElementOrderDtoValidator : AbstractValidator<WBSElementOrderDto>
{
    public WBSElementOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Element ID is required");

        RuleFor(x => x.SequenceNumber)
            .GreaterThan(0).WithMessage("Sequence number must be greater than 0");
    }
}