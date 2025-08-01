using Core.DTOs.WBS;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for ConvertPlanningToWorkPackageDto
/// </summary>
public class ConvertPlanningToWorkPackageDtoValidator : AbstractValidator<ConvertPlanningToWorkPackageDto>
{
    public ConvertPlanningToWorkPackageDtoValidator()
    {
        RuleFor(x => x.ProgressMethod)
            .IsInEnum().WithMessage("Invalid progress method");

        RuleFor(x => x.PlannedStartDate)
            .NotEmpty().WithMessage("Planned start date is required")
            .Must(BeAValidDate).WithMessage("Invalid planned start date");

        RuleFor(x => x.PlannedEndDate)
            .NotEmpty().WithMessage("Planned end date is required")
            .Must(BeAValidDate).WithMessage("Invalid planned end date")
            .GreaterThan(x => x.PlannedStartDate).WithMessage("Planned end date must be after start date");

        RuleFor(x => x.Budget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be greater than or equal to 0");

        RuleFor(x => x.WeightFactor)
            .InclusiveBetween(0, 100).When(x => x.WeightFactor.HasValue)
            .WithMessage("Weight factor must be between 0 and 100");
    }

    private bool BeAValidDate(DateTime date)
    {
        return date >= DateTime.Today && date <= DateTime.Today.AddYears(20);
    }
}
