using FluentValidation;
using ProductServices.Domain.Dto;

namespace ProductServices.Domain.Validacion.Producto
{
    public class ProductoUpdateValidacion : AbstractValidator<ProductoUpdateDto>
    {
        public ProductoUpdateValidacion()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El Nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El Nombre no puede tener más de 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La Descripción es obligatoria.")
                .MaximumLength(500).WithMessage("La Descripción no puede tener más de 500 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

        }

    }
}
