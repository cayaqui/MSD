using Core.Constants;
using Core.DTOs.Organization.Project;
using FluentValidation;

namespace Application.Validators.Auth;

public class TransferTeamMemberDtoValidator : AbstractValidator<TransferTeamMemberDto>
{
    public TransferTeamMemberDtoValidator()
    {
        RuleFor(x => x.NewProjectId)
            .NotEmpty().WithMessage("New project ID is required");

        RuleFor(x => x.TransferDate)
            .NotEmpty().WithMessage("Transfer date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Transfer date cannot be in the past");

        RuleFor(x => x.NewRole)
            .Must(BeValidRole).When(x => !string.IsNullOrEmpty(x.NewRole))
            .WithMessage("Invalid role");

        RuleFor(x => x.TransferReason)
            .MaximumLength(500).WithMessage("Transfer reason must not exceed 500 characters");
    }

    private bool BeValidRole(string? role)
    {
        if (string.IsNullOrEmpty(role)) return true;
        return ProjectRoles.AllRoles.Contains(role);
    }
}