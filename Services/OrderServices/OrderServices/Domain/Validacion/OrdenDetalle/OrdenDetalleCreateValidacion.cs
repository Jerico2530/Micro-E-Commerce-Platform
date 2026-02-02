using FluentValidation;
using OrderServices.Domain.Dto.OrdenDetalle;

namespace OrderServices.Domain.Validacion.OrdenDetalle
{
    public class OrdenDetalleCreateValidacion : AbstractValidator<OrdenDetalleCreateDto>
    {
        public OrdenDetalleCreateValidacion()
        {
            RuleFor(x => x.ProductoId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0)
                .WithMessage("La cantidad debe ser mayor a 0.");
        }
    }
}
