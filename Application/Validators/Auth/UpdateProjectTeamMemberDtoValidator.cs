using Core.Constants;
using Core.DTOs.Auth.ProjectTeamMembers;
using FluentValidation;

namespace Application.Validators.Auth;

public class UpdateProjectTeamMemberDtoValidator : AbstractValidator<UpdateProjectTeamMemberDto>
{
    public UpdateProjectTeamMemberDtoValidator()
    {
        RuleFor(x => x.Role)
            .Must(BeValidRole).When(x => !string.IsNullOrEmpty(x.Role))
            .WithMessage("Invalid role. Must be one of: ProjectManager, TeamLead, TeamMember, or Viewer");

        RuleFor(x => x.AllocationPercentage)
            .InclusiveBetween(0, 100).When(x => x.AllocationPercentage.HasValue)
            .WithMessage("Allocation percentage must be between 0 and 100");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate ?? DateTime.UtcNow)
            .When(x => x.EndDate.HasValue && x.StartDate.HasValue)
            .WithMessage("End date must be after start date");
    }

    private bool BeValidRole(string? role)
    {
        if (string.IsNullOrEmpty(role)) return true;
        return ProjectRoles.AllRoles.Contains(role);
    }
}
