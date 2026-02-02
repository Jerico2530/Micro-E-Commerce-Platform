using FluentValidation;
using IdentityServices.Domain.Dto.Rol;

namespace IdentityServices.Domain.Validacion.Rol
{
    public class RolCreateValidacion : AbstractValidator<RolCreateDto>
    {
        public RolCreateValidacion()
        {
            RuleFor(x => x.NombreRol)
                .NotEmpty().WithMessage("ElNombre Rol  es obligatorio.")
                .MaximumLength(50).WithMessage("Máximo 50 caracteres.");
        }
    }
}
