using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderServices.Infrastructure.Datos;
using OrderServices.Infrastructure.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OrderServices.Infrastructure.Repositories
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
      
            private readonly AppDbContext _db;
            internal DbSet<T> dbSet;

            public Repositorio(AppDbContext db)
            {
                _db = db;
                this.dbSet = _db.Set<T>();
            }

            public async Task Crear(T entidad)
            {
                await dbSet.AddAsync(entidad);
                await Grabar();
            }
            public async Task Grabar()
            {
                await _db.SaveChangesAsync();
            }

            public async Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true, Func<IQueryable<T>, IQueryable<T>> include = null)
            {
                IQueryable<T> query = dbSet;
                if (!tracked)
                {
                    query = query.AsNoTracking();
                }

                if (include != null)
                {
                    query = include(query);
                }

                if (filtro != null)
                {
                    query = query.Where(filtro);

                }
                return await query.FirstOrDefaultAsync();
            }

            public async Task<List<T>> ObtenerTodo(Expression<Func<T, bool>>? filtro = null)
            {
                IQueryable<T> query = dbSet;
                if (filtro != null)
                {
                    query = query.Where(filtro);

                }
                return await query.ToListAsync();

            }

            public async Task Remover(T entidad)
            {
                dbSet.Remove(entidad);
                await Grabar();
            }

            public async Task<bool> Existe(Expression<Func<T, bool>> filtro)
            {
                return await dbSet.AnyAsync(filtro);
            }
            public async Task ActualizarVariosAsync(IEnumerable<T> entidades)
            {
                dbSet.UpdateRange(entidades);
                await _db.SaveChangesAsync();
            }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _db.Database.BeginTransactionAsync();
        }

    }
}
