using Core.DTOs.WBS;
using Core.Enums.Projects;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for CreateWBSElementDto
/// </summary>
public class CreateWBSElementDtoValidator : AbstractValidator<CreateWBSElementDto>
{
    public CreateWBSElementDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("WBS code is required")
            .MaximumLength(50).WithMessage("WBS code cannot exceed 50 characters")
            .Matches(@"^[0-9]+(\.[0-9]+)*$").WithMessage("WBS code must be in format X.Y.Z (e.g., 1.2.3)");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.ElementType)
            .IsInEnum().WithMessage("Invalid element type");

        RuleFor(x => x.SequenceNumber)
            .GreaterThan(0).When(x => x.SequenceNumber.HasValue)
            .WithMessage("Sequence number must be greater than 0");

        // Custom validation
        RuleFor(x => x)
            .Must(BeValidElementTypeForLevel)
            .WithMessage("Invalid element type for the specified level")
            .WithName("ElementType");
    }

    private bool BeValidElementTypeForLevel(CreateWBSElementDto dto)
    {
        // If creating at root level (no parent), cannot be Work Package or Planning Package
        if (!dto.ParentId.HasValue &&
            (dto.ElementType == WBSElementType.WorkPackage ||
             dto.ElementType == WBSElementType.PlanningPackage))
        {
            return false;
        }

        return true;
    }
}
