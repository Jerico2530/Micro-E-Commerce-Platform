using FluentValidation;

namespace IdentityServices.Domain.Validacion.Rol
{
    public class RolGetValidacion : AbstractValidator<int>
    {
        public RolGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del rol debe ser válido.");
        }
    }
}
