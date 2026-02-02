using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;

namespace IdentityServices.Infrastructure.Repositories
{
    public class PermisoRepositorio : Repositorio<Permiso>, IPermisoRepositorio
    {
        private readonly AppDbContext _db; // Contexto principal de persistencia de datos

        public PermisoRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un permiso existente en la base de datos
        public async Task<Permiso> ActualizarPermiso(Permiso entidad)
        {
            _db.Permisos.Update(entidad);     // Marca la entidad como modificada para persistir cambios
            await _db.SaveChangesAsync();    // Guarda los cambios en la base de datos
            return entidad;                  // Retorna la entidad actualizada
        }
    }

}
