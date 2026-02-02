using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.PermRol;

namespace IdentityServices.Application.Services.IServices
{
    public interface IPermRolService
    {
        // Obtiene todos los permisos asignados a roles, incluyendo detalles de cada relación
        Task<ApiResponse<List<PermRolDto>>> ObtenerPermRolConDetallesAsync();

        // Crea una nueva relación Permiso-Rol
        Task<ApiResponse<PermRolDto>> CrearPermRolAsync(PermRolCreateDto dto);

        // Obtiene una relación Permiso-Rol específica por su identificador
        Task<ApiResponse<PermRolDto>> ObtenerPermRolPorIdAsync(int id);

        // Actualiza una relación Permiso-Rol existente reemplazando su información
        Task<ApiResponse<PermRolDto>> ActualizarPermRolAsync(int id, PermRolUpdateDto updateDto);

        // Elimina una relación Permiso-Rol del sistema
        Task<ApiResponse<object>> EliminarPermRolAsync(int id);

    }
}
