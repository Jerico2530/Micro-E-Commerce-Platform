using FluentValidation;
using IdentityServices.Domain.Dto.Usuario;

namespace IdentityServices.Domain.Validacion.Usuario
{
    public class UsuarioUpdateValidacion : AbstractValidator<UsuarioUpdateDto>
    {
        public UsuarioUpdateValidacion()
        {
            RuleFor(x => x.NombreCompleto)
                .MaximumLength(100).WithMessage("El Nombre Completo no puede tener más de 100 caracteres.");

            RuleFor(x => x.ApellidoCompleto)

                .MaximumLength(100).WithMessage("El Apellido Completo no puede tener más de 100 caracteres.");

            RuleFor(x => x.Email)
                     .MaximumLength(100).WithMessage("El correo electrónico no puede tener más de 100 caracteres.")
                     .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        }
    }
}
