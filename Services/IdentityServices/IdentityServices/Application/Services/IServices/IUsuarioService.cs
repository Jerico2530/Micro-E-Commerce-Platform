using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.Usuario;
using Microsoft.AspNetCore.JsonPatch;

namespace IdentityServices.Application.Services.IServices
{
    public interface IUsuarioService
    {
        // Obtiene todos los usuarios registrados en el sistema
        Task<ApiResponse<List<UsuarioDto>>> ObtenerTodosLosUsuarioAsync();

        // Obtiene un usuario específico por su identificador
        Task<ApiResponse<UsuarioDto>> ObtenerUsuarioPorIdAsync(int id);

        // Obtiene la información del usuario actualmente autenticado
        Task<ApiResponse<UsuarioDto>> ObtenerUsuarioActualAsync(int userId);

        // Crea un nuevo usuario en el sistema
        Task<ApiResponse<UsuarioDto>> CrearUsuarioAsync(UsuarioCreateDto dto);

        // Actualiza completamente la información de un usuario existente
        Task<ApiResponse<UsuarioDto>> ActualizarUsuarioAsync(int id, UsuarioUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un usuario usando JsonPatch
        Task<ApiResponse<UsuarioDto>> ActualizarParcialUsuarioAsync(int id, JsonPatchDocument<UsuarioUpdateDto> patchDto);

        // Elimina un usuario del sistema
        Task<ApiResponse<object>> EliminarUsuarioAsync(int id);


    }
}
