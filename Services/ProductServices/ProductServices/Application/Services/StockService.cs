using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using ProductServices.Api.Helpers;
using ProductServices.Api.Responses;
using ProductServices.Application.Services.IServices;
using ProductServices.Domain.Dto;
using ProductServices.Domain.Entities;
using ProductServices.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace ProductServices.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IProductoRepositorio _productoRepo;
        private readonly ILogger<StockService> _logger;

        public StockService(IProductoRepositorio productoRepo, ILogger<StockService> logger)
        {
            _productoRepo = productoRepo;
            _logger = logger;
        }

        public async Task<ApiResponse<StockResponseDto>> ConsultarStockAsync(int productoId)
        {
            try
            {
                if (productoId <= 0)
                    return ResponseHelper.Fail<StockResponseDto>("ID inválido", "ProductoId");

                var producto = await _productoRepo.Obtener(p => p.ProductoId == productoId, tracked: false);

                if (producto == null)
                {
                    _logger.LogWarning("⚠️ Producto con ID {Id} no encontrado.", productoId);
                    return ResponseHelper.Fail<StockResponseDto>(
                        "Producto no encontrado.",
                        "ProductoId",
                        HttpStatusCode.NotFound
                    );
                }

                var response = new StockResponseDto
                {
                    ProductoId = producto.ProductoId,
                    StockActual = producto.Stock,
                    Disponible = producto.Stock > 0
                };

                _logger.LogInformation("✅ Stock de Producto ID {Id} consultado correctamente.", productoId);
                return ResponseHelper.Success(response, "Stock consultado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al consultar stock Producto ID {Id}", productoId);
                return ResponseHelper.FailException<StockResponseDto>(ex);
            }
        }

        public async Task<ApiResponse<StockResponseDto>> ReducirStockAsync(StockRequestDto dto)
        {
            try
            {
                if (dto == null || dto.Cantidad <= 0)
                    return ResponseHelper.Fail<StockResponseDto>("Cantidad inválida.", "Cantidad");

                var producto = await _productoRepo.Obtener(p => p.ProductoId == dto.ProductoId, tracked: true);

                if (producto == null)
                {
                    _logger.LogWarning("⚠️ Producto con ID {Id} no encontrado.", dto.ProductoId);
                    return ResponseHelper.Fail<StockResponseDto>(
                        "Producto no encontrado.",
                        "ProductoId",
                        HttpStatusCode.NotFound
                    );
                }

                if (producto.Stock < dto.Cantidad)
                {
                    _logger.LogWarning("⚠️ Stock insuficiente para Producto ID {Id}. Cantidad solicitada: {Cantidad}, Stock actual: {Stock}",
                        dto.ProductoId, dto.Cantidad, producto.Stock);
                    return ResponseHelper.Fail<StockResponseDto>(
                        "Stock insuficiente.",
                        "Stock",
                        HttpStatusCode.Conflict
                    );
                }

                producto.Stock -= dto.Cantidad;
                await _productoRepo.ActualizarProducto(producto);

                var response = new StockResponseDto
                {
                    ProductoId = producto.ProductoId,
                    StockActual = producto.Stock,
                    Disponible = producto.Stock > 0
                };

                _logger.LogInformation("✅ Stock reducido Producto ID {Id} en {Cantidad}. Nuevo stock: {Stock}",
                    dto.ProductoId, dto.Cantidad, producto.Stock);

                return ResponseHelper.Success(response, "Stock reducido correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al reducir stock Producto ID {Id}", dto?.ProductoId);
                return ResponseHelper.FailException<StockResponseDto>(ex);
            }
        }
    }
}
