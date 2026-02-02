
using FluentValidation;
using IdentityServices.Domain.Dto.Permiso;

namespace IdentityServices;

public class PermisoUpdateValidacion : AbstractValidator<PermisoUpdateDto>
{
    public PermisoUpdateValidacion()
    {
        RuleFor(x => x.NombrePermiso)
             .NotEmpty().WithMessage("El Nombre Permiso  es obligatorio.")
             .MaximumLength(100).WithMessage("El Nombre Permiso no puede tener más de 100 caracteres.");
    }
}
