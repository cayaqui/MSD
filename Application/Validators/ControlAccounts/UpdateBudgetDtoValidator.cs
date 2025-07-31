using FluentValidation;
using Core.DTOs.Budget;

namespace Application.Validators.ControlAccounts;

public class UpdateBudgetDtoValidator : AbstractValidator<UpdateBudgetDto>
{
    public UpdateBudgetDtoValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");
        });

        When(x => x.TotalAmount.HasValue, () =>
        {
            RuleFor(x => x.TotalAmount!.Value)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0");
        });

        When(x => x.ContingencyAmount.HasValue, () =>
        {
            RuleFor(x => x.ContingencyAmount!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Contingency amount cannot be negative");
        });

        When(x => x.ManagementReserve.HasValue, () =>
        {
            RuleFor(x => x.ManagementReserve!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Management reserve cannot be negative");
        });

        When(x => x.ExchangeRate.HasValue, () =>
        {
            RuleFor(x => x.ExchangeRate!.Value)
                .GreaterThan(0).WithMessage("Exchange rate must be greater than 0");
        });
    }
}
