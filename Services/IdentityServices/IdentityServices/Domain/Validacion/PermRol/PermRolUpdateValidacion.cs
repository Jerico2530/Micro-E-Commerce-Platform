
using FluentValidation;
using IdentityServices.Domain.Dto.PermRol;

namespace IdentityServices;

public class PermRolUpdateValidacion : AbstractValidator<PermRolUpdateDto>
{
    public PermRolUpdateValidacion()
    {
        RuleFor(x => x.Estado)
                 .NotNull().WithMessage("El estado es obligatorio.")
                 .Must(v => v == true || v == false)
                 .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
    }
}
