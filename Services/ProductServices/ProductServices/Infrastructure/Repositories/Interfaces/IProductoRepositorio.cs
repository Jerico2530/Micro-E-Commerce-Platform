using ProductServices.Domain.Entities;

namespace ProductServices.Infrastructure.Repositories.Interfaces
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {

        /// Actualiza un Producto existente y devuelve la entidad actualizada.
        Task<Producto> ActualizarProducto(Producto entidad);
        Task<bool> ReducirStockAsync(int productoId, int cantidad);


    }
}
