using Core.DTOs.Auth.Users;
using FluentValidation;

namespace Application.Validators.Auth;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.EntraId)
            .NotEmpty().WithMessage("EntraId is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.GivenName)
            .MaximumLength(100).WithMessage("Given name must not exceed 100 characters");

        RuleFor(x => x.Surname)
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters");

        RuleFor(x => x.PreferredLanguage)
            .MaximumLength(10).WithMessage("Preferred language must not exceed 10 characters");
    }
}
