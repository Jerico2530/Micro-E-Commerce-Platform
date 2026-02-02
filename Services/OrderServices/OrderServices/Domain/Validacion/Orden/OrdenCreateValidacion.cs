using FluentValidation;
using OrderServices.Domain.Dto.Orden;

namespace OrderServices.Domain.Validacion.Orden
{
    public class OrdenCreateValidacion : AbstractValidator<OrdenCreateDto>
    {
        public OrdenCreateValidacion()
        {
            RuleFor(x => x.Items)
               .NotEmpty()
               .WithMessage("Debe agregar al menos un producto a la orden.");

            RuleForEach(x => x.Items).SetValidator(new OrderServices.Domain.Validacion.OrdenDetalle.OrdenDetalleCreateValidacion());

        }
    }
}
