using Core.DTOs.Organization.Project;
using FluentValidation;

namespace Application.Validators.Setup;

// Validators
public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters");

        RuleFor(x => x.OperationId)
            .NotEmpty().WithMessage("Operation is required");

        RuleFor(x => x.PlannedStartDate)
            .NotEmpty().WithMessage("Planned start date is required");

        RuleFor(x => x.PlannedEndDate)
            .NotEmpty().WithMessage("Planned end date is required")
            .GreaterThan(x => x.PlannedStartDate).WithMessage("End date must be after start date");

        RuleFor(x => x.TotalBudget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be non-negative");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");
    }
}
