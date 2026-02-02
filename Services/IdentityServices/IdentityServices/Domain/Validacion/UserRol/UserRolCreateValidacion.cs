using FluentValidation;
using IdentityServices.Domain.Dto.UserRol;

namespace IdentityServices.Domain.Validacion.UserRol
{
    public class UserRolCreateValidacion : AbstractValidator<UserRolCreateDto>
    {
        public UserRolCreateValidacion()
        {
            RuleFor(x => x.RolId)
                .GreaterThan(0).WithMessage("Debe seleccionar un rol válido.");

            RuleFor(x => x.UsuarioId)
                .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");
        }
    }
}