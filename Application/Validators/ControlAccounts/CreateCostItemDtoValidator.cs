using Core.DTOs.Cost.CostItems;
using FluentValidation;

namespace Application.Validators.ControlAccounts;

public class CreateCostItemDtoValidator : AbstractValidator<CreateCostItemDto>
{
    public CreateCostItemDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.ItemCode)
            .NotEmpty().WithMessage("Item code is required")
            .MaximumLength(50).WithMessage("Item code must not exceed 50 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid cost type");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid cost category");

        RuleFor(x => x.PlannedCost)
            .GreaterThanOrEqualTo(0).WithMessage("Planned cost cannot be negative");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters");
    }
}
