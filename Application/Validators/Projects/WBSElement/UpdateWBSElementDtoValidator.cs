using FluentValidation;
using Core.DTOs.Projects.WBSElement;

namespace Application.Validators.Projects.WBSElement;

/// <summary>
/// Validator for UpdateWBSElementDto
/// </summary>
public class UpdateWBSElementDtoValidator : AbstractValidator<UpdateWBSElementDto>
{
    public UpdateWBSElementDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("La descripción no puede exceder 500 caracteres");
    }
}