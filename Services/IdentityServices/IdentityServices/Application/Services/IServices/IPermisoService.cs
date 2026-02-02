using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.Permiso;
using Microsoft.AspNetCore.JsonPatch;

namespace IdentityServices.Application.Services.IServices
{
    public interface IPermisoService
    {
        // Obtiene todos los permisos registrados en el sistema
        Task<ApiResponse<List<PermisoDto>>> ObtenerTodosLosPermisoAsync();

        // Obtiene un permiso específico por su identificador
        Task<ApiResponse<PermisoDto>> ObtenerPermisoPorIdAsync(int id);

        // Registra un nuevo permiso en el sistema
        Task<ApiResponse<PermisoDto>> CrearPermisoAsync(PermisoCreateDto dto);

        // Actualiza un permiso existente reemplazando su información
        Task<ApiResponse<PermisoDto>> ActualizarPermisoAsync(int id, PermisoUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un permiso usando JsonPatch
        Task<ApiResponse<PermisoDto>> ActualizarParcialPermisoAsync(int id, JsonPatchDocument<PermisoUpdateDto> patchDto);

        // Elimina un permiso del sistema
        Task<ApiResponse<object>> EliminarPermisoAsync(int id);

    }

}
