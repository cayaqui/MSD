using FluentValidation;
using Core.DTOs.Projects.WBSElement;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for ReorderWBSElementsDto
/// </summary>
public class ReorderWBSElementsDtoValidator : AbstractValidator<ReorderWBSElementsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReorderWBSElementsDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Elements)
            .NotEmpty().WithMessage("Debe especificar al menos un elemento para reordenar")
            .Must(HaveUniqueIds).WithMessage("Los IDs de elementos deben ser únicos")
            .Must(HaveUniqueSequences).WithMessage("Los números de secuencia deben ser únicos");

        RuleForEach(x => x.Elements)
            .ChildRules(element =>
            {
                element.RuleFor(e => e.Id)
                    .NotEmpty().WithMessage("El ID del elemento es requerido")
                    .MustAsync(BeValidWBSElement).WithMessage("El elemento WBS especificado no existe");

                element.RuleFor(e => e.SequenceNumber)
                    .GreaterThan(0).WithMessage("El número de secuencia debe ser mayor que cero");
            });
    }

    private bool HaveUniqueIds(List<WBSElementOrderDto> elements)
    {
        return elements.Select(e => e.Id).Distinct().Count() == elements.Count;
    }

    private bool HaveUniqueSequences(List<WBSElementOrderDto> elements)
    {
        return elements.Select(e => e.SequenceNumber).Distinct().Count() == elements.Count;
    }

    private async Task<bool> BeValidWBSElement(Guid id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository<Domain.Entities.WBS.WBSElement>()
            .Query()
            .AnyAsync(w => w.Id == id, cancellationToken);
    }
}