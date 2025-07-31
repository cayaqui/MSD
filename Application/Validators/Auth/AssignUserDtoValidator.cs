using FluentValidation;

namespace Application.Validators.Auth;

public class AssignUserDtoValidator : AbstractValidator<AssignUserDto>
{
    public AssignUserDtoValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(BeValidRole).WithMessage("Invalid role");

        RuleFor(x => x.AllocationPercentage)
            .InclusiveBetween(0, 100).When(x => x.AllocationPercentage.HasValue)
            .WithMessage("Allocation percentage must be between 0 and 100");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).When(x => x.EndDate.HasValue)
            .WithMessage("End date must be after start date");
    }

    private bool BeValidRole(string role)
    {
        return ProjectRoles.AllRoles.Contains(role);
    }
}
