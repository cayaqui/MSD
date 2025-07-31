using FluentValidation;

namespace Application.Validators.ControlAccounts;

public class RecordActualCostDtoValidator : AbstractValidator<RecordActualCostDto>
{
    public RecordActualCostDtoValidator()
    {
        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(0).WithMessage("Actual cost cannot be negative");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Transaction date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Transaction date cannot be in the future");
    }
}
