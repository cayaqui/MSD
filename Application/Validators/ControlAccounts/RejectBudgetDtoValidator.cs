using FluentValidation;
using Core.DTOs.Budget;

namespace Application.Validators.ControlAccounts;

public class RejectBudgetDtoValidator : AbstractValidator<RejectBudgetDto>
{
    public RejectBudgetDtoValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}
