using FluentValidation;
using Core.DTOs.EVM;

namespace Application.Validators.ControlAccounts;

public class CreateEVMRecordDtoValidator : AbstractValidator<CreateEVMRecordDto>
{
    public CreateEVMRecordDtoValidator()
    {
        RuleFor(x => x.ControlAccountId)
            .NotEmpty().WithMessage("Control Account ID is required");

        RuleFor(x => x.DataDate)
            .NotEmpty().WithMessage("Data date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Data date cannot be in the future");

        RuleFor(x => x.PeriodType)
            .IsInEnum().WithMessage("Invalid period type");

        RuleFor(x => x.PV)
            .GreaterThanOrEqualTo(0).WithMessage("Planned Value cannot be negative");

        RuleFor(x => x.EV)
            .GreaterThanOrEqualTo(0).WithMessage("Earned Value cannot be negative");

        RuleFor(x => x.AC)
            .GreaterThanOrEqualTo(0).WithMessage("Actual Cost cannot be negative");

        RuleFor(x => x.BAC)
            .GreaterThan(0).WithMessage("Budget at Completion must be greater than 0");

        RuleFor(x => x.EV)
            .LessThanOrEqualTo(x => x.PV)
            .WithMessage("Earned Value cannot exceed Planned Value");
    }
}
