using FluentValidation;
using OrderServices.Domain.Dto.Orden;

namespace OrderServices.Domain.Validacion.Orden
{
    public class OrdenUpdateValidacion : AbstractValidator<OrdenUpdateDto>
    {
        public OrdenUpdateValidacion()
        {

            RuleFor(x => x.EstadoOrden)
                .NotEmpty()
                .WithMessage("El estado de la orden es obligatorio.");

        }
    }
}
