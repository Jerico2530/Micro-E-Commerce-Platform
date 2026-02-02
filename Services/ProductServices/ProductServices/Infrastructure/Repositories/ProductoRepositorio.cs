using Microsoft.EntityFrameworkCore;
using ProductServices.Domain.Entities;
using ProductServices.Infrastructure.Datos;
using ProductServices.Infrastructure.Repositories.Interfaces;

namespace ProductServices.Infrastructure.Repositories
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly AppDbContext _db; // Contexto de acceso a datos del sistema

        public ProductoRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un producto existente en la base de datos.
        public async Task<Producto> ActualizarProducto(Producto entidad)
        {
            _db.Productos.Update(entidad); // Marca la entidad como modificada para persistir cambios
            await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return entidad;               // Devuelve la entidad actualizada
        }

        public async Task<bool> ReducirStockAsync(int productoId, int cantidad)
        {
            var producto = await _db.Productos
    .SingleOrDefaultAsync(p => p.ProductoId == productoId);


            if (producto == null) return false;
            if (producto.Stock < cantidad) return false;

            producto.Stock -= cantidad;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false; // conflicto de concurrencia
            }
        }

    }
}

