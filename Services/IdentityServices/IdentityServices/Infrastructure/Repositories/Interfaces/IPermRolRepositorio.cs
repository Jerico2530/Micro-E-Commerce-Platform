using IdentityServices.Domain.Entities;

namespace IdentityServices.Infrastructure.Repositories.Interfaces
{
    public interface IPermRolRepositorio : IRepositorio<PermRol>
    {
        /// Actualiza una relación PermRol existente y devuelve la entidad actualizada.
        Task<PermRol> ActualizarPermRol(PermRol entidad);
        /// Obtiene todas las relaciones PermRol con sus detalles completos.
        Task<List<PermRol>> ObtenerPermRolConDetalles();
        /// Obtiene una relación PermRol específica por su ID con detalles completos.
        Task<PermRol?> ObtenerPermRolConDetallesPorId(int id);

    }
}
