using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.Rol;
using Microsoft.AspNetCore.JsonPatch;

namespace IdentityServices.Application.Services.IServices
{
    public interface IRolService
    {
        // Obtiene todos los roles registrados en el sistema
        Task<ApiResponse<List<RolDto>>> ObtenerTodosLosRolAsync();

        // Obtiene un rol específico según su identificador
        Task<ApiResponse<RolDto>> ObtenerRolPorIdAsync(int id);

        // Crea un nuevo rol en el sistema
        Task<ApiResponse<RolDto>> CrearRolAsync(RolCreateDto dto);

        // Actualiza completamente un rol existente
        Task<ApiResponse<RolDto>> ActualizarRolAsync(int id, RolUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un rol usando JsonPatch
        Task<ApiResponse<RolDto>> ActualizarParcialRolAsync(int id, JsonPatchDocument<RolUpdateDto> patchDto);

        // Elimina un rol del sistema
        Task<ApiResponse<object>> EliminarRolAsync(int id);


    }
}
