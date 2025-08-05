using Core.DTOs.Projects.WBSElement;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for UpdateWBSDictionaryDto
/// </summary>
public class UpdateWBSDictionaryDtoValidator : AbstractValidator<UpdateWBSDictionaryDto>
{
    public UpdateWBSDictionaryDtoValidator()
    {
        RuleFor(x => x.DeliverableDescription)
            .MaximumLength(2000).WithMessage("Deliverable description cannot exceed 2000 characters");

        RuleFor(x => x.AcceptanceCriteria)
            .MaximumLength(2000).WithMessage("Acceptance criteria cannot exceed 2000 characters");

        RuleFor(x => x.Assumptions)
            .MaximumLength(2000).WithMessage("Assumptions cannot exceed 2000 characters");

        RuleFor(x => x.Constraints)
            .MaximumLength(2000).WithMessage("Constraints cannot exceed 2000 characters");

        RuleFor(x => x.ExclusionsInclusions)
            .MaximumLength(2000).WithMessage("Exclusions/Inclusions cannot exceed 2000 characters");
    }
}
