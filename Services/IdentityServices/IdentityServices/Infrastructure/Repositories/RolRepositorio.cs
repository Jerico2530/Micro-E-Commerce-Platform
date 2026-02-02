using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;

namespace IdentityServices.Infrastructure.Repositories
{
    public class RolRepositorio : Repositorio<Rol>, IRolRepositorio
    {
        private readonly AppDbContext _db;// Contexto de acceso a la base de datos

        public RolRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un rol existente en la base de datos.
        public async Task<Rol> ActualizarRol(Rol entidad)
        {
            _db.Roles.Update(entidad);        // Registra la entidad como modificada para su actualización
            await _db.SaveChangesAsync();   // Persiste los cambios en la base de datos
            return entidad;                 // Retorna la entidad ya actualizada
        }
    }
}

