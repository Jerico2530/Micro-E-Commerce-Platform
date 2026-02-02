using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Entities;
using OrderServices.Infrastructure.Datos;
using OrderServices.Infrastructure.Repositories.Interfaces;

namespace OrderServices.Infrastructure.Repositories
{
    public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        public readonly AppDbContext _db; // Contexto de base de datos para la persistencia

        public OrdenRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza una orden existente en la base de datos.
        public async Task<Orden> ActualizarOrden(Orden entidad)
        {
            _db.Ordenes.Update(entidad);         // Marca la entidad como modificada
            await _db.SaveChangesAsync();      // Persiste los cambios
            return entidad;                    // Devuelve la orden actualizada
        }

    }
}
