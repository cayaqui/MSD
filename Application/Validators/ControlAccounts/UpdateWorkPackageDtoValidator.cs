using FluentValidation;
using Core.DTOs.WorkPackages;

namespace Application.Validators.ControlAccounts;

public class UpdateWorkPackageDtoValidator : AbstractValidator<UpdateWorkPackageDto>
{
    public UpdateWorkPackageDtoValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Description), () =>
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        });

        When(x => x.Budget.HasValue, () =>
        {
            RuleFor(x => x.Budget!.Value)
                .GreaterThan(0).WithMessage("Budget must be greater than 0");
        });

        When(x => x.PlannedStartDate.HasValue && x.PlannedEndDate.HasValue, () =>
        {
            RuleFor(x => x.PlannedEndDate!.Value)
                .GreaterThan(x => x.PlannedStartDate!.Value)
                .WithMessage("End date must be after start date");
        });

        When(x => x.WeightFactor.HasValue, () =>
        {
            RuleFor(x => x.WeightFactor!.Value)
                .InclusiveBetween(0, 1)
                .WithMessage("Weight factor must be between 0 and 1");
        });
    }
}
