using FluentValidation;

namespace OrderServices.Domain.Validacion.Orden
{
    public class OrdenGetValidacion : AbstractValidator<int>
    {
        public OrdenGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del orden debe ser válido.");
        }
    }
}
