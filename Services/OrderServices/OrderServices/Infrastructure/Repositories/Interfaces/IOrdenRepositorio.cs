using OrderServices.Domain.Entities;

namespace OrderServices.Infrastructure.Repositories.Interfaces
{
    public interface IOrdenRepositorio : IRepositorio<Orden>
    {
        /// Actualiza una orden existente y devuelve la entidad actualizada.
        Task<Orden> ActualizarOrden(Orden entidad);

    }

}
