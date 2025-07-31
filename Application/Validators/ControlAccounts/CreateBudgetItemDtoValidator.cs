using FluentValidation;
using Core.DTOs.Budget;

namespace Application.Validators.ControlAccounts;

public class CreateBudgetItemDtoValidator : AbstractValidator<CreateBudgetItemDto>
{
    public CreateBudgetItemDtoValidator()
    {
        RuleFor(x => x.BudgetId)
            .NotEmpty().WithMessage("Budget ID is required");

        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required")
            .MaximumLength(50).WithMessage("Item code must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.CostType)
            .IsInEnum().WithMessage("Invalid cost type");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid cost category");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitRate)
            .GreaterThanOrEqualTo(0).WithMessage("Unit rate cannot be negative");
    }
}
