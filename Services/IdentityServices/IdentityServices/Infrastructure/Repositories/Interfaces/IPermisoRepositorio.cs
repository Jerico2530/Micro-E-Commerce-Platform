using IdentityServices.Domain.Entities;

namespace IdentityServices.Infrastructure.Repositories.Interfaces
{
    public interface IPermisoRepositorio : IRepositorio<Permiso>
    {
        /// Actualiza un permiso existente y devuelve la entidad actualizada.
        Task<Permiso> ActualizarPermiso(Permiso entidad);
    }
}
