using FluentValidation;
using Core.DTOs.WorkPackages;

namespace Application.Validators.ControlAccounts;

public class CreateWorkPackageDtoValidator : AbstractValidator<CreateWorkPackageDto>
{
    public CreateWorkPackageDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(30).WithMessage("Code must not exceed 30 characters")
            .Matches(@"^P-\d{3}-\d{2}-\d{3}-\d{2}$")
            .WithMessage("Code must follow format: P-XXX-YY-ZZZ-WW");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.ControlAccountId)
            .NotEmpty().WithMessage("Control Account ID is required");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(x => x.PlannedEndDate)
            .GreaterThan(x => x.PlannedStartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x.ProgressMethod)
            .IsInEnum().WithMessage("Invalid progress method");

        When(x => x.WeightFactor.HasValue, () =>
        {
            RuleFor(x => x.WeightFactor!.Value)
                .InclusiveBetween(0, 1)
                .WithMessage("Weight factor must be between 0 and 1");
        });
    }
}
