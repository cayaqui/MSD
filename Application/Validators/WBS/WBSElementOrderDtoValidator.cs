using Core.DTOs.Projects.WBSElement;
using Core.Enums.Progress;
using FluentValidation;

namespace Application.Validators.WBS;

/// <summary>
/// Validator for WBSElementOrderDto
/// </summary>
public class WBSElementOrderDtoValidator : AbstractValidator<WBSElementOrderDto>
{
    public WBSElementOrderDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID del elemento es requerido");

        RuleFor(x => x.SequenceNumber)
            .GreaterThan(0).WithMessage("El número de secuencia debe ser mayor que 0");
    }
}