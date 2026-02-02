using ProductServices.Api.Responses;
using ProductServices.Domain.Dto;

namespace ProductServices.Application.Services.IServices
{
    public interface IStockService
    {
        Task<ApiResponse<StockResponseDto>> ConsultarStockAsync(int productoId);
        Task<ApiResponse<StockResponseDto>> ReducirStockAsync(StockRequestDto dto);
    }
}
