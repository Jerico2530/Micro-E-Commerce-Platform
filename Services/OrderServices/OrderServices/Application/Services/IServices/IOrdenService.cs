using Microsoft.AspNetCore.JsonPatch;
using OrderServices.Api.Responses;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Dto.OrdenDetalle;

namespace OrderServices.Application.Services.IServices
{
    public interface IOrdenService
    {

        // Obtiene todas las órdenes registradas en el sistema
        Task<ApiResponse<List<OrdenDto>>> ObtenerTodosLosOrdenAsync();

        // Obtiene una orden específica por su identificador
        Task<ApiResponse<OrdenDto>> ObtenerOrdenPorIdAsync(int id);

        // Crea una nueva orden en el sistema
        Task<ApiResponse<OrdenDto>> CrearOrdenAsync(OrdenCreateDto dto, int usuarioId, string token);

        // Actualiza una orden existente reemplazando su información
        Task<ApiResponse<OrdenDto>> ActualizarOrdenAsync(int id, OrdenUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre una orden usando JsonPatch
        Task<ApiResponse<OrdenDto>> ActualizarParcialOrdenAsync(int id, JsonPatchDocument<OrdenUpdateDto> patchDto);

        // Elimina una orden del sistema
        Task<ApiResponse<object>> EliminarOrdenAsync(int id);

    }
}
