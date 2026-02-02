using OrderServices.Domain.Entities;

namespace OrderServices.Infrastructure.Repositories.Interfaces
{
    public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
    {
        /// Actualiza un detalle de orden existente y devuelve la entidad actualizada.
        Task<OrdenDetalle> ActualizarOrdenDetalle(OrdenDetalle entidad);
    }
}
