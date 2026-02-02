using System.Linq.Expressions;

namespace ProductServices.Infrastructure.Repositories.Interfaces
{
    public interface IRepositorio<T> where T : class
    {
        /// Crea una nueva entidad en el repositorio.
        Task Crear(T entidad);
        /// Obtiene todas las entidades que cumplan un filtro opcional.
        Task<List<T>> ObtenerTodo(Expression<Func<T, bool>>? filtro = null);
        /// Obtiene una entidad específica según el filtro, con opción de tracking y carga de relaciones.
        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true, Func<IQueryable<T>, IQueryable<T>> include = null);
        /// Verifica si existe alguna entidad que cumpla con el filtro.
        Task<bool> Existe(Expression<Func<T, bool>> filtro);
        /// Actualiza varias entidades en una operación.
        Task ActualizarVariosAsync(IEnumerable<T> entidades);
        /// Elimina una entidad del repositorio.
        Task Remover(T entidad);
        /// Persiste los cambios pendientes en la base de datos.
        Task Grabar();
    }
}
