using FluentValidation;

namespace IdentityServices.Domain.Validacion.Usuario
{
    public class UsuarioGetValidacion : AbstractValidator<int>
    {
        public UsuarioGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del usuario debe ser válido.");
        }
    }

}
