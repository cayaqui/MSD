using Core.DTOs.Organization.Company;
using FluentValidation;

namespace Application.Validators.Setup;

// Validators
public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
{
    public CreateCompanyDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(20).WithMessage("Code must not exceed 20 characters")
            .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("Code can only contain letters, numbers, and hyphens");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("Tax ID is required")
            .MaximumLength(50).WithMessage("Tax ID must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.DefaultCurrency)
            .NotEmpty().WithMessage("Default currency is required")
            .Length(3).WithMessage("Currency must be 3 characters")
            .Matches(@"^[A-Z]{3}$").WithMessage("Currency must be in ISO 4217 format (e.g., USD, EUR)");
    }
}
