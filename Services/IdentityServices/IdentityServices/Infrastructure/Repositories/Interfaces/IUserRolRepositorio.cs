using IdentityServices.Domain.Entities;

namespace IdentityServices.Infrastructure.Repositories.Interfaces
{
    public interface IUserRolRepositorio : IRepositorio<UserRol>
    {
        /// Actualiza un registro de UserRol existente en la base de datos.
        Task<UserRol> ActualizarUserRol(UserRol entidad);
        /// Obtiene todos los registros de UserRol incluyendo detalles relacionados.
        Task<List<UserRol>> ObtenerUserRolesConDetalles();
        /// Obtiene un registro de UserRol específico con detalles asociados por su ID.
        Task<UserRol?> ObtenerUserRolConDetallesPorId(int id);
    }
}
