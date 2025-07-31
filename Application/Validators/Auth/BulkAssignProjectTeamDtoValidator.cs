using FluentValidation;

namespace Application.Validators.Auth;

public class BulkAssignProjectTeamDtoValidator : AbstractValidator<BulkAssignProjectTeamDto>
{
    public BulkAssignProjectTeamDtoValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("Project ID is required");

        RuleFor(x => x.Users)
            .NotEmpty().WithMessage("At least one user must be specified")
            .Must(HaveUniqueUserIds).WithMessage("Duplicate user IDs found");

        RuleForEach(x => x.Users).SetValidator(new AssignUserDtoValidator());
    }

    private bool HaveUniqueUserIds(List<AssignUserDto> users)
    {
        if (users == null || !users.Any()) return true;
        var userIds = users.Select(u => u.UserId).ToList();
        return userIds.Count == userIds.Distinct().Count();
    }
}
