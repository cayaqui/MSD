using Core.DTOs.Organization.Company;
using FluentValidation;

namespace Application.Validators.Setup;

public class UpdateCompanyDtoValidator : AbstractValidator<UpdateCompanyDto>
{
    public UpdateCompanyDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.TaxId)
            .MaximumLength(50).WithMessage("Tax ID must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.TaxId));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Website)
            .Must(BeAValidUrl).WithMessage("Invalid website URL")
            .When(x => !string.IsNullOrEmpty(x.Website));

        RuleFor(x => x.Phone)
            .Matches(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{4,6}$")
            .WithMessage("Invalid phone number format")
            .When(x => !string.IsNullOrEmpty(x.Phone));
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}