using FluentValidation;

namespace OrderServices.Domain.Validacion.Orden
{
    public class OrdenDeleteValidacion : AbstractValidator<int>
    {
        public OrdenDeleteValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del orden debe ser válido para eliminar.");
        }
    }
}
