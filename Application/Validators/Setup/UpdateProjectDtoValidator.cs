using Core.DTOs.Projects;
using FluentValidation;

namespace Application.Validators.Setup;

public class UpdateProjectDtoValidator : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => x.Description != null);

        RuleFor(x => x.TotalBudget)
            .GreaterThanOrEqualTo(0).WithMessage("Budget must be non-negative")
            .When(x => x.TotalBudget.HasValue);

        When(x => x.PlannedStartDate.HasValue && x.PlannedEndDate.HasValue, () =>
        {
            RuleFor(x => x.PlannedEndDate)
                .GreaterThan(x => x.PlannedStartDate!.Value)
                .WithMessage("End date must be after start date");
        });

        RuleFor(x => x.Status)
            .Must(BeAValidStatus).WithMessage("Invalid project status")
            .When(x => !string.IsNullOrEmpty(x.Status));
    }

    private bool BeAValidStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return true;
        var validStatuses = new[] { "Planning", "Active", "OnHold", "Completed", "Cancelled", "Delayed", "Closed" };
        return validStatuses.Contains(status);
    }
}