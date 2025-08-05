using Core.DTOs.Cost;
using FluentValidation;

namespace Application.Validators.ControlAccounts;

public class RecordCommitmentDtoValidator : AbstractValidator<RecordCommitmentDto>
{
    public RecordCommitmentDtoValidator()
    {
        RuleFor(x => x.CommittedCost)
            .GreaterThanOrEqualTo(0).WithMessage("Committed cost cannot be negative");

        RuleFor(x => x.ReferenceType)
            .NotEmpty().WithMessage("Reference type is required")
            .MaximumLength(50).WithMessage("Reference type must not exceed 50 characters");

        RuleFor(x => x.ReferenceNumber)
            .NotEmpty().WithMessage("Reference number is required")
            .MaximumLength(100).WithMessage("Reference number must not exceed 100 characters");
    }
}
