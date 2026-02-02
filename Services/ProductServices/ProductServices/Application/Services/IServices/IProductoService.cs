using Microsoft.AspNetCore.JsonPatch;
using ProductServices.Api.Responses;
using ProductServices.Domain.Dto;

namespace ProductServices.Application.Services.IServices
{
    public interface IProductoService
    {
        // Obtiene todos los productos registrados en el sistema
        Task<ApiResponse<List<ProductoDto>>> ObtenerTodosLosProductosAsync();

        // Obtiene un producto específico por su identificador
        Task<ApiResponse<ProductoDto>> ObtenerProductoPorIdAsync(int id);

        // Crea un nuevo producto en el sistema
        Task<ApiResponse<ProductoDto>> CrearProductoAsync(ProductoCreateDto dto);

        // Actualiza completamente un producto existente
        Task<ApiResponse<ProductoDto>> ActualizarProductoAsync(int id, ProductoUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un producto usando JsonPatch
        Task<ApiResponse<ProductoDto>> ActualizarParcialProductoAsync(int id, JsonPatchDocument<ProductoUpdateDto> patchDto);

        // Elimina un producto del sistema
        Task<ApiResponse<object>> EliminarProductoAsync(int id);

    }
}
