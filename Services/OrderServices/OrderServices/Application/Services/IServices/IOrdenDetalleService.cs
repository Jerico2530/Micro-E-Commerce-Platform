using Microsoft.AspNetCore.JsonPatch;
using OrderServices.Api.Responses;
using OrderServices.Domain.Dto.OrdenDetalle;

namespace OrderServices.Application.Services.IServices
{
    public interface IOrdenDetalleService
    {
        // Obtiene todos los detalles de órdenes registrados en el sistema
        Task<ApiResponse<List<OrdenDetalleDto>>> ObtenerTodosLosOrdenDetalleAsync();

        // Obtiene un detalle de orden específico por su identificador
        Task<ApiResponse<OrdenDetalleDto>> ObtenerOrdenDetallePorIdAsync(int id);

        // Crea un nuevo detalle de orden en el sistema
        Task<ApiResponse<OrdenDetalleDto>> CrearOrdenDetalleAsync(OrdenDetalleCreateDto dto);

        // Actualiza un detalle de orden existente reemplazando su información
        Task<ApiResponse<OrdenDetalleDto>> ActualizarOrdenDetalleAsync(int id, OrdenDetalleUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un detalle de orden usando JsonPatch
        Task<ApiResponse<OrdenDetalleDto>> ActualizarParcialOrdenDetalleAsync(int id, JsonPatchDocument<OrdenDetalleUpdateDto> patchDto);

        // Elimina un detalle de orden del sistema
        Task<ApiResponse<object>> EliminarOrdenDetalleAsync(int id);


    }

}
