using FluentValidation;
using IdentityServices.Domain.Dto.Usuario;

namespace IdentityServices.Domain.Validacion.Usuario
{
    public class LoginCreateValidacion : AbstractValidator<UsuarioLoginDto>
    {
        public LoginCreateValidacion()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo es obligatorio")
                .EmailAddress().WithMessage("Formato de correo inválido")
                .Matches(@"^[\w.+-]+@gmail\.com$").WithMessage("Solo se permiten correos @gmail.com");

            RuleFor(x => x.Contraseña)
                .NotEmpty().WithMessage("La contraseña es obligatoria");

        }
    }
}
