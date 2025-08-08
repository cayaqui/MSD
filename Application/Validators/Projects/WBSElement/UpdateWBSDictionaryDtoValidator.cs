using FluentValidation;
using Core.DTOs.Projects.WBSElement;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for UpdateWBSDictionaryDto
/// </summary>
public class UpdateWBSDictionaryDtoValidator : AbstractValidator<UpdateWBSDictionaryDto>
{
    public UpdateWBSDictionaryDtoValidator()
    {
        RuleFor(x => x.DeliverableDescription)
            .MaximumLength(2000).WithMessage("La descripción del entregable no puede exceder 2000 caracteres");

        RuleFor(x => x.AcceptanceCriteria)
            .MaximumLength(2000).WithMessage("Los criterios de aceptación no pueden exceder 2000 caracteres");

        RuleFor(x => x.Assumptions)
            .MaximumLength(2000).WithMessage("Los supuestos no pueden exceder 2000 caracteres");

        RuleFor(x => x.Constraints)
            .MaximumLength(2000).WithMessage("Las restricciones no pueden exceder 2000 caracteres");

        RuleFor(x => x.ExclusionsInclusions)
            .MaximumLength(2000).WithMessage("Las exclusiones/inclusiones no pueden exceder 2000 caracteres");
    }
}