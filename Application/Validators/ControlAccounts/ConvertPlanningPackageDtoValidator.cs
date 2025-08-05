using Core.DTOs.Cost.PlanningPackages;
using FluentValidation;

namespace Application.Validators.ControlAccounts;

public class ConvertPlanningPackageDtoValidator : AbstractValidator<ConvertPlanningPackageDto>
{
    public ConvertPlanningPackageDtoValidator()
    {
        RuleFor(x => x.WorkPackages)
            .NotEmpty().WithMessage("At least one work package is required");

        RuleForEach(x => x.WorkPackages).SetValidator(new CreateWorkPackageFromPlanningDtoValidator());

        RuleFor(x => x.WorkPackages)
            .Must(workPackages => workPackages.Sum(wp => wp.Budget) > 0)
            .WithMessage("Total work package budget must be greater than 0");
    }
}
