using FluentValidation;
using Core.DTOs.Cost.ControlAccounts;

namespace Application.Validators.ControlAccounts;

public class CreateControlAccountDtoValidator : AbstractValidator<CreateControlAccountDto>
{
    public CreateControlAccountDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(20).WithMessage("Code must not exceed 20 characters")
            .Matches(@"^C-\d{3}-\d{2}-[A-Z]{3}-\d{2}$")
            .WithMessage("Code must follow format: C-XXX-YY-CAM-##");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.PhaseId)
            .NotEmpty().WithMessage("Phase ID is required");

        RuleFor(x => x.CAMUserId)
            .NotEmpty().WithMessage("Control Account Manager is required");

        RuleFor(x => x.BAC)
            .GreaterThan(0).WithMessage("Budget at Completion must be greater than 0");

        RuleFor(x => x.ContingencyReserve)
            .GreaterThanOrEqualTo(0).WithMessage("Contingency Reserve cannot be negative");

        RuleFor(x => x.ManagementReserve)
            .GreaterThanOrEqualTo(0).WithMessage("Management Reserve cannot be negative");

        RuleFor(x => x.MeasurementMethod)
            .IsInEnum().WithMessage("Invalid measurement method");
    }
}
