using FluentValidation;

namespace ProductServices.Domain.Validacion.Producto
{
    public class ProductoGetValidacion : AbstractValidator<int>
    {
        public ProductoGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del producto debe ser válido.");
        }
    }
}
