
using FluentValidation;
using IdentityServices.Domain.Dto.PermRol;

namespace IdentityServices;

public class PermRolCreateValidacion : AbstractValidator<PermRolCreateDto>
{
    public PermRolCreateValidacion()
    {
        RuleFor(x => x.PermisoId)
           .GreaterThan(0).WithMessage("Debe seleccionar un permiso válido.");

        RuleFor(x => x.RolId)
           .GreaterThan(0).WithMessage("Debe seleccionar un rol válido.");

    }
}
