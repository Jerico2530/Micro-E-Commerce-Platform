using FluentValidation;
using IdentityServices.Domain.Dto.UserRol;

namespace IdentityServices.Domain.Validacion.UserRol
{
    public class UserRolUpdateValidacion : AbstractValidator<UserRolUpdateDto>
    {
        public UserRolUpdateValidacion()
        {
            RuleFor(x => x.Estado)
                    .NotNull().WithMessage("El estado es obligatorio.")
                    .Must(v => v == true || v == false)
                    .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
        }
    }
}
