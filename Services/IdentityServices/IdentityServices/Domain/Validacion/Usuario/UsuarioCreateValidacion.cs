using FluentValidation;
using IdentityServices.Domain.Dto.Usuario;

namespace IdentityServices.Domain.Validacion.Usuario
{
    public class UsuarioCreateValidacion : AbstractValidator<UsuarioCreateDto>
    {
        public UsuarioCreateValidacion()
        {
            RuleFor(x => x.NombreCompleto)
                 .NotEmpty().WithMessage("El Nombre Completo  es obligatorio.")
                 .MaximumLength(100).WithMessage("El Nombre Completo no puede tener más de 100 caracteres.");

            RuleFor(x => x.ApellidoCompleto)
                .NotEmpty().WithMessage("ElApellido Completo  es obligatorio.")
                .MaximumLength(100).WithMessage("El Apellido Completo no puede tener más de 100 caracteres.");

            RuleFor(x => x.Email)
                     .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                     .MaximumLength(100).WithMessage("El correo electrónico no puede tener más de 100 caracteres.")
                     .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

            RuleFor(x => x.Contraseña)
               .NotEmpty().WithMessage("El Contraseña  es obligatorio.")
                .MaximumLength(100).WithMessage("El Contraseña no puede tener más de 100 caracteres.");

            RuleFor(x => x.ContraseñaVisible)
                .NotEmpty().WithMessage("El Contraseña   es obligatorio.")
                .MaximumLength(100).WithMessage("El Contraseña  no puede tener más de 100 caracteres.");
        }
    }
}
