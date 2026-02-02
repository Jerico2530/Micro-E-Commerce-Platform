using FluentValidation;
using OrderServices.Domain.Dto.OrdenDetalle;

namespace OrderServices.Domain.Validacion.OrdenDetalle
{
    public class OrdenDetalleUpdateValidacion : AbstractValidator<OrdenDetalleUpdateDto>
    {
        public OrdenDetalleUpdateValidacion()
        {
            RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0.");

        }
    }
}
