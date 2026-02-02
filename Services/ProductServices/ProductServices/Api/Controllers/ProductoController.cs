using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ProductServices.Api.Responses;
using ProductServices.Application.Services.IServices;
using ProductServices.Domain.Dto;
using ProductServices.Infrastructure.Security;

namespace ProductServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _ProductoService;

        public ProductoController(ILogger<ProductoController> logger, IProductoService productoService)
        {
            _logger = logger;
            _ProductoService = productoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission("Producto.Ver")]

        public async Task<ActionResult<ApiResponse<List<ProductoDto>>>> GetProducto()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Productos");
            // Llama la capa de servicios para obtener el listado de productos.
            var response = await _ProductoService.ObtenerTodosLosProductosAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission("Producto.VerDetalle")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> GetProducto(int id)
        {
            _logger.LogInformation("🔍 Solicitando Producto con ID {ProductoId}.", id);
            // Consulta al servicio por el detalle de la producto solicitada.
            var response = await _ProductoService.ObtenerProductoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequirePermission("Producto.Crear")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> CrearProducto([FromBody] ProductoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Producto.");
            // Solicita la creación de la producto en la capa de servicios.
            var response = await _ProductoService.CrearProductoAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Producto: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetProducto", new { id = carrito?.ProductoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequirePermission("Producto.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarProducto(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Producto con ID {ProductoId}", id);
            // Solicita al servicio eliminar la producto indicada.
            var response = await _ProductoService.EliminarProductoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Producto.Actualizar")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> ActualizarProducto(int id, [FromBody] ProductoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando producto con ID {Id}.", id);
            // Solicita la actualización completa de la producto.
            var response = await _ProductoService.ActualizarProductoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Producto.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> UpdateParcialProducto(int id, [FromBody] JsonPatchDocument<ProductoUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Producto con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _ProductoService.ActualizarParcialProductoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}

