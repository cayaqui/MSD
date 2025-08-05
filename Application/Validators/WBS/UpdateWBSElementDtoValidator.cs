using Core.DTOs.Projects.WBSElement;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for UpdateWBSElementDto
/// </summary>
public class UpdateWBSElementDtoValidator : AbstractValidator<UpdateWBSElementDto>
{
    public UpdateWBSElementDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
    }
}
