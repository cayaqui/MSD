using FluentValidation;
using Core.DTOs.Projects.WorkPackageDetails;
using Domain.Interfaces;
using Domain.Entities.Cost.Control;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for ConvertToWorkPackageDto
/// </summary>
public class ConvertToWorkPackageDtoValidator : AbstractValidator<ConvertToWorkPackageDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConvertToWorkPackageDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ControlAccountId)
            .NotEmpty().WithMessage("La cuenta de control es requerida")
            .MustAsync(BeValidControlAccount).WithMessage("La cuenta de control especificada no existe");

        RuleFor(x => x.ProgressMethod)
            .IsInEnum().WithMessage("El método de progreso no es válido");

        RuleFor(x => x.ResponsibleUserId)
            .MustAsync(BeValidUser).WithMessage("El usuario responsable especificado no existe")
            .When(x => x.ResponsibleUserId.HasValue);
    }

    private async Task<bool> BeValidControlAccount(Guid controlAccountId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<ControlAccount>()
            .Query()
            .AnyAsync(ca => ca.Id == controlAccountId, cancellationToken);
    }

    private async Task<bool> BeValidUser(Guid? userId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return true;

        return await _unitOfWork.Repository<Domain.Entities.Auth.Security.User>()
            .Query()
            .AnyAsync(u => u.Id == userId.Value, cancellationToken);
    }
}