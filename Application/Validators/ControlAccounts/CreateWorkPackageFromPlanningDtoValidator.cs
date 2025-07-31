using FluentValidation;
using Core.DTOs.Cost;
using Core.Enums.Cost;
using Core.Enums.Progress;

namespace Application.Validators.ControlAccounts;

public class CreateWorkPackageFromPlanningDtoValidator : AbstractValidator<CreateWorkPackageFromPlanningDto>
{
    public CreateWorkPackageFromPlanningDtoValidator()
    {
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
    }
}