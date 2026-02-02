using IdentityServices.Api.Dto;
using IdentityServices.Api.Responses;
using IdentityServices.Domain.Dto.Usuario;

namespace IdentityServices.Application.Services.IServices
{
    public interface ILoginService
    {

        // Autentica un usuario registrado con sus credenciales y devuelve información de sesión
        Task<ApiResponse<LoginResultDto>> LoginAsync(UsuarioLoginDto loginDto);

        // Permite el acceso temporal como invitado, sin necesidad de registro
        Task<ApiResponse<LoginResultDto>> LoginInvitadoAsync();

    }
}
