using FluentValidation;
using Core.DTOs.WorkPackages;

namespace Application.Validators.ControlAccounts;

public class UpdateWorkPackageProgressDtoValidator : AbstractValidator<UpdateWorkPackageProgressDto>
{
    public UpdateWorkPackageProgressDtoValidator()
    {
        RuleFor(x => x.ProgressPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Progress percentage must be between 0 and 100");

        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Actual cost cannot be negative");

        RuleFor(x => x.CommittedCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Committed cost cannot be negative");

        RuleFor(x => x.ProgressDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Progress date cannot be in the future");
    }
}
