using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityServices.Infrastructure.Repositories
{
    public class PermRolRepositorio : Repositorio<PermRol>, IPermRolRepositorio
    {
        private readonly AppDbContext _db;  // Contexto principal de persistencia de datos

        public PermRolRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza una relación Permiso-Rol existente.
        public async Task<PermRol> ActualizarPermRol(PermRol entidad)
        {
            _db.PermRoles.Update(entidad);  // Marca la entidad como modificada para persistir cambios
            await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return entidad;               // Devuelve la entidad actualizada
        }
        /// Obtiene todas las relaciones Permiso-Rol con los datos asociados de cada tabla.
        public async Task<List<PermRol>> ObtenerPermRolConDetalles()
        {
            return await _db.PermRoles
                .Include(ur => ur.Permiso)
                .Include(ur => ur.Rol)
                .ToListAsync();
        }
        /// Obtiene una relación Permiso-Rol específica por Id con sus detalles asociados.
        public async Task<PermRol?> ObtenerPermRolConDetallesPorId(int id)
        {
            return await _db.PermRoles
                .Include(ur => ur.Permiso)
                .Include(ur => ur.Rol)
                .FirstOrDefaultAsync(ur => ur.PermRolId == id);
        }

    }
}
