using IdentityServices.Domain.Entities;

namespace IdentityServices.Infrastructure.Repositories.Interfaces
{
    public interface IRolRepositorio : IRepositorio<Rol>
    {
        /// Actualiza una entidad Rol existente en la base de datos.
        Task<Rol> ActualizarRol(Rol entidad);
    }
}
