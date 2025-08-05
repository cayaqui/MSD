using Core.DTOs.Cost.PlanningPackages;
using FluentValidation;

namespace Application.Validators.ControlAccounts;

public class CreatePlanningPackageDtoValidator : AbstractValidator<CreatePlanningPackageDto>
{
    public CreatePlanningPackageDtoValidator()
    {
        RuleFor(x => x.ControlAccountId)
            .NotEmpty().WithMessage("Control Account ID is required");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(30).WithMessage("Code must not exceed 30 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(x => x.PlannedEndDate)
            .GreaterThan(x => x.PlannedStartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");
    }
}
