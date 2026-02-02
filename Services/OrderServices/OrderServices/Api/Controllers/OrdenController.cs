using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OrderServices.Api.Helpers;
using OrderServices.Api.Responses;
using OrderServices.Application.Helpers;
using OrderServices.Application.Services.IServices;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Dto.OrdenDetalle;
using OrderServices.Infrastructure.Security;
using System.Net;

namespace OrderServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly ILogger<OrdenController> _logger;
        private readonly IOrdenService _OrdenService;

        public OrdenController(ILogger<OrdenController> logger, IOrdenService OrdenService)
        {
            _logger = logger;
            _OrdenService = OrdenService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [RequirePermission("Orden.Ver")]

        public async Task<ActionResult<ApiResponse<List<OrdenDto>>>> GetOrden()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Ordens");
            // Llama la capa de servicios para obtener el listado de ordens.
            var response = await _OrdenService.ObtenerTodosLosOrdenAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetOrden")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequirePermission("Orden.VerDetalle")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> GetOrden(int id)
        {
            _logger.LogInformation("🔍 Solicitando Orden con ID {OrdenId}.", id);
            // Consulta al servicio por el detalle de la orden solicitada.
            var response = await _OrdenService.ObtenerOrdenPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequirePermission("Orden.Crear")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> CrearOrden([FromBody] OrdenCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nueva Orden.");

            int usuarioId = User.GetUsuarioId();
            if (usuarioId == 0)
            {
                _logger.LogWarning("⚠️ No se pudo identificar el usuario desde el token.");
                return BadRequest(ResponseHelper.Fail<OrdenDto>("No se pudo identificar el usuario desde el token.", "UsuarioId", HttpStatusCode.BadRequest));
            }

            string token = Request.GetBearerToken();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("⚠️ No se encontró token de autorización.");
                return BadRequest(ResponseHelper.Fail<OrdenDto>("No se encontró token de autorización.", "Authorization", HttpStatusCode.BadRequest));
            }

            var response = await _OrdenService.CrearOrdenAsync(createDto, usuarioId, token);

            if (!response.IsExitoso)
            {
                _logger.LogWarning("⚠️ Error al crear Orden: {@Response}", response);
                return StatusCode((int)response.StatusCode, response);
            }

            var orden = response.Resultado;
            return CreatedAtRoute("GetOrden", new { id = orden?.OrdenId }, response);
        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequirePermission("Orden.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarOrden(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Orden con ID {OrdenId}", id);
            // Solicita al servicio eliminar la orden indicada.
            var response = await _OrdenService.EliminarOrdenAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Orden.Actualizar")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> ActualizarOrden(int id, [FromBody] OrdenUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando orden con ID {Id}.", id);
            // Solicita la actualización completa de la orden.
            var response = await _OrdenService.ActualizarOrdenAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [RequirePermission("Orden.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> UpdateParcialOrden(int id, [FromBody] JsonPatchDocument<OrdenUpdateDto> patchDto)
        {

            _logger.LogInformation("🧩 Actualización parcial de Orden con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _OrdenService.ActualizarParcialOrdenAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}


