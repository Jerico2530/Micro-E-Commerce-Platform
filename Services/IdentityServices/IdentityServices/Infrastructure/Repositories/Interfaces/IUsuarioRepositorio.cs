using IdentityServices.Domain.Entities;

namespace IdentityServices.Infrastructure.Repositories.Interfaces
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {

        /// Actualiza un Usuario existente y devuelve la entidad actualizada.
        Task<Usuario> ActualizarUsuario(Usuario entidad);


    }
}
