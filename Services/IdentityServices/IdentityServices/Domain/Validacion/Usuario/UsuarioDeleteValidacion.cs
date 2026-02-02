using FluentValidation;

namespace IdentityServices.Domain.Validacion.Usuario
{
    public class UsuarioDeleteValidacion : AbstractValidator<int>
    {
        public UsuarioDeleteValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del usuario debe ser válido para eliminar.");
        }
    }
}

