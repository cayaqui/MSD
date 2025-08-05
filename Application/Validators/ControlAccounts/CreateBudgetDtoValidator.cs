using FluentValidation;
using Core.DTOs.Cost.Budgets;

namespace Application.Validators.ControlAccounts;

public class CreateBudgetDtoValidator : AbstractValidator<CreateBudgetDto>
{
    public CreateBudgetDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Version)
            .NotEmpty().WithMessage("Version is required")
            .MaximumLength(20).WithMessage("Version must not exceed 20 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid budget type");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.ContingencyAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Contingency amount cannot be negative");

        RuleFor(x => x.ManagementReserve)
            .GreaterThanOrEqualTo(0).WithMessage("Management reserve cannot be negative");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");

        RuleFor(x => x.ExchangeRate)
            .GreaterThan(0).WithMessage("Exchange rate must be greater than 0");
    }
}
