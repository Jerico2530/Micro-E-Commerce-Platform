using OrderServices.Domain.Entities;
using OrderServices.Infrastructure.Datos;
using OrderServices.Infrastructure.Repositories.Interfaces;

namespace OrderServices.Infrastructure.Repositories
{
    public class OrdenDetalleRepositorio : Repositorio<OrdenDetalle>, IOrdenDetalleRepositorio
    {
        public readonly AppDbContext _db;  // Contexto EF Core para acceso a la base de datos

        public OrdenDetalleRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un detalle de orden existente en la base de datos
        public async Task<OrdenDetalle> ActualizarOrdenDetalle(OrdenDetalle entidad)
        {
            _db.OrdenDetalles.Update(entidad); // Marca la entidad como modificada
            await _db.SaveChangesAsync(); // Persiste los cambios en la base de datos
            return entidad; // Retorna la entidad actualizada
        }
    }
}
