using Microsoft.AspNetCore.Mvc;
using ProductServices.Api.Responses;
using ProductServices.Application.Services.IServices;
using ProductServices.Domain.Dto;
using ProductServices.Infrastructure.Security;

namespace ProductServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ILogger<StockController> _logger;
        private readonly IStockService _stockService;

        public StockController(ILogger<StockController> logger, IStockService stockService)
        {
            _logger = logger;
            _stockService = stockService;
        }

        /// <summary>
        /// Consulta el stock de un producto por su ID.
        /// </summary>
        [HttpGet("{productoId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Stock.Ver")]
        public async Task<ActionResult<ApiResponse<StockResponseDto>>> GetStock(int productoId)
        {
            _logger.LogInformation("📢 Solicitud para consultar stock del Producto ID {ProductoId}", productoId);

            var response = await _stockService.ConsultarStockAsync(productoId);

            return StatusCode((int)response.StatusCode, response);
        }

        /// <summary>
        /// Reduce el stock de un producto.
        /// </summary>
        [HttpPost("reducir")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Stock.Crear")]
        public async Task<ActionResult<ApiResponse<StockResponseDto>>> ReducirStock([FromBody] StockRequestDto dto)
        {
            _logger.LogInformation("📝 Solicitud para reducir stock del Producto ID {ProductoId} en {Cantidad}", dto?.ProductoId, dto?.Cantidad);

            var response = await _stockService.ReducirStockAsync(dto);

            if (!response.IsExitoso)
                _logger.LogWarning("⚠️ Error al reducir stock: {@Response}", response);

            return StatusCode((int)response.StatusCode, response);
        }
    }
}