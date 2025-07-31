using FluentValidation;

namespace Application.Validators.Auth;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.GivenName)
            .MaximumLength(100).WithMessage("Given name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.GivenName));

        RuleFor(x => x.Surname)
            .MaximumLength(100).WithMessage("Surname must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Surname));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters")
            .Matches(@"^[\d\s\-\+\(\)]+$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.JobTitle)
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.JobTitle));

        RuleFor(x => x.PreferredLanguage)
            .MaximumLength(10).WithMessage("Preferred language must not exceed 10 characters")
            .When(x => !string.IsNullOrEmpty(x.PreferredLanguage));
    }
}