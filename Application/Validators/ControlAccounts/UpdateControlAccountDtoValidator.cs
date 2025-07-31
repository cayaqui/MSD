using FluentValidation;
using Core.DTOs.ControlAccounts;

namespace Application.Validators.ControlAccounts;

public class UpdateControlAccountDtoValidator : AbstractValidator<UpdateControlAccountDto>
{
    public UpdateControlAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        When(x => x.BAC.HasValue, () =>
        {
            RuleFor(x => x.BAC!.Value)
                .GreaterThan(0).WithMessage("Budget at Completion must be greater than 0");
        });

        When(x => x.ContingencyReserve.HasValue, () =>
        {
            RuleFor(x => x.ContingencyReserve!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Contingency Reserve cannot be negative");
        });

        When(x => x.ManagementReserve.HasValue, () =>
        {
            RuleFor(x => x.ManagementReserve!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Management Reserve cannot be negative");
        });

        When(x => x.MeasurementMethod.HasValue, () =>
        {
            RuleFor(x => x.MeasurementMethod!.Value)
                .IsInEnum().WithMessage("Invalid measurement method");
        });
    }
}
