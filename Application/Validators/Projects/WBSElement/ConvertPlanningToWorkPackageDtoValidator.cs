using FluentValidation;
using Core.DTOs.Projects.WorkPackageDetails;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for ConvertPlanningToWorkPackageDto
/// </summary>
public class ConvertPlanningToWorkPackageDtoValidator : AbstractValidator<ConvertPlanningToWorkPackageDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConvertPlanningToWorkPackageDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ProgressMethod)
            .IsInEnum().WithMessage("El método de progreso no es válido");

        RuleFor(x => x.ResponsibleUserId)
            .MustAsync(BeValidUser).WithMessage("El usuario responsable especificado no existe")
            .When(x => x.ResponsibleUserId.HasValue);

        RuleFor(x => x.PrimaryDisciplineId)
            .MustAsync(BeValidDiscipline).WithMessage("La disciplina especificada no existe")
            .When(x => x.PrimaryDisciplineId.HasValue);
    }

    private async Task<bool> BeValidUser(Guid? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return true;

        return await _unitOfWork.Repository<Domain.Entities.Auth.Security.User>()
            .Query()
            .AnyAsync(u => u.Id == userId.Value, cancellationToken);
    }

    private async Task<bool> BeValidDiscipline(Guid? disciplineId, CancellationToken cancellationToken)
    {
        if (!disciplineId.HasValue) return true;

        return await _unitOfWork.Repository<Domain.Entities.Organization.Core.Discipline>()
            .Query()
            .AnyAsync(d => d.Id == disciplineId.Value, cancellationToken);
    }
}