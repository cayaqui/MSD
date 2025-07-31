using FluentValidation;
using Core.DTOs.WorkPackages;

namespace Application.Validators.ControlAccounts;

public class CreateActivityDtoValidator : AbstractValidator<CreateActivityDto>
{
    public CreateActivityDtoValidator()
    {
        RuleFor(x => x.ActivityCode)
            .NotEmpty().WithMessage("Activity code is required")
            .MaximumLength(50).WithMessage("Activity code must not exceed 50 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.PlannedEndDate)
            .GreaterThan(x => x.PlannedStartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.PlannedHours)
            .GreaterThan(0).WithMessage("Planned hours must be greater than 0");
    }
}
