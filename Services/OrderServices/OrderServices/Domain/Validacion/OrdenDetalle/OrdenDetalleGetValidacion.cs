using FluentValidation;

namespace OrderServices.Domain.Validacion.OrdenDetalle
{
    public class OrdenDetalleGetValidacion : AbstractValidator<int>
    {
        public OrdenDetalleGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del orden detallado debe ser válido.");
        }
    }
}
