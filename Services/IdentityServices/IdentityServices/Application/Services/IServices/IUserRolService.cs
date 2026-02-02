using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.UserRol;

namespace IdentityServices.Application.Services.IServices
{
    public interface IUserRolService
    {

        // Obtiene todas las relaciones usuario-rol con detalles
        Task<ApiResponse<List<UserRolDto>>> ObtenerUserRolesConDetallesAsync();

        // Obtiene una relación específica según su identificador
        Task<ApiResponse<UserRolDto>> ObtenerUserRolPorIdAsync(int id);

        // Crea una nueva relación entre usuario y rol
        Task<ApiResponse<UserRolDto>> CrearUserRolAsync(UserRolCreateDto dto);

        // Elimina una relación usuario-rol existente
        Task<ApiResponse<object>> EliminarUserRolAsync(int id);

        // Actualiza completamente una relación usuario-rol
        Task<ApiResponse<UserRolDto>> ActualizarUserRolAsync(int id, UserRolUpdateDto updateDto);


    }
}
