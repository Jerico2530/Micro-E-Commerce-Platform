using FluentValidation;

namespace IdentityServices;

public class PermRolGetValidacion : AbstractValidator<int>
{
    public PermRolGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del permiso-rol debe ser válido.");
    }
}
