using FluentValidation;
using Core.DTOs.EVM;

namespace Application.Validators.ControlAccounts;

public class UpdateEVMActualsDtoValidator : AbstractValidator<UpdateEVMActualsDto>
{
    public UpdateEVMActualsDtoValidator()
    {
        RuleFor(x => x.EV)
            .GreaterThanOrEqualTo(0).WithMessage("Earned Value cannot be negative");

        RuleFor(x => x.AC)
            .GreaterThanOrEqualTo(0).WithMessage("Actual Cost cannot be negative");
    }
}
