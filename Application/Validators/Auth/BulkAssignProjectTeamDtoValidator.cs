using Core.DTOs.Organization.Project;
using FluentValidation;

namespace Application.Validators.Auth;

public class BulkAssignProjectTeamDtoValidator : AbstractValidator<BulkAssignProjectTeamDto>
{
    public BulkAssignProjectTeamDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Assignments)
            .NotEmpty().WithMessage("At least one assignment must be specified")
            .Must(HaveUniqueUserIds).WithMessage("Duplicate user IDs found");

        RuleForEach(x => x.Assignments).SetValidator(new AssignProjectTeamMemberDtoValidator());
    }

    private bool HaveUniqueUserIds(List<AssignProjectTeamMemberDto> assignments)
    {
        if (assignments == null || !assignments.Any()) return true;
        var userIds = assignments.Select(a => a.UserId).ToList();
        return userIds.Count == userIds.Distinct().Count();
    }
}
