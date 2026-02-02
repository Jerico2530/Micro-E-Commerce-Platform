using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;

namespace IdentityServices.Infrastructure.Repositories
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly AppDbContext _db; // Contexto de acceso a datos del sistema

        public UsuarioRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un Usuario existente en la base de datos.
        public async Task<Usuario> ActualizarUsuario(Usuario entidad)
        {
            _db.Usuarios.Update(entidad); // Marca la entidad como modificada para persistir cambios
            await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return entidad;               // Devuelve la entidad actualizada
        }
    }
}
