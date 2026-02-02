using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Permiso;
using IdentityServices.Security.Attributes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServices.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermisoController : ControllerBase
    {
        private readonly ILogger<PermisoController> _logger;
        private readonly IPermisoService _PermisoService;

        public PermisoController(ILogger<PermisoController> logger, IPermisoService PermisoService)
        {
            _logger = logger;
            _PermisoService = PermisoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Permiso.Ver")]
        public async Task<ActionResult<ApiResponse<List<PermisoDto>>>> GetPermiso()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Permisos");
            // Llama la capa de servicios para obtener el listado de permisos.
            var response = await _PermisoService.ObtenerTodosLosPermisoAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetPermiso")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Permiso.VerDetalle")]

        public async Task<ActionResult<ApiResponse<PermisoDto>>> GetPermiso(int id)
        {
            _logger.LogInformation("🔍 Solicitando Permiso con ID {PermisoId}.", id);
            // Consulta al servicio por el detalle de la permiso solicitada.
            var response = await _PermisoService.ObtenerPermisoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Permiso.Crear")]
        public async Task<ActionResult<ApiResponse<PermisoDto>>> CrearPermiso([FromBody] PermisoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Permiso.");
            // Solicita la creación de la permiso en la capa de servicios.
            var response = await _PermisoService.CrearPermisoAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Permiso: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetPermiso", new { id = carrito?.PermisoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Permiso.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarPermiso(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Permiso con ID {PermisoId}", id);
            // Solicita al servicio eliminar la permiso indicada.
            var response = await _PermisoService.EliminarPermisoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Permiso.Actualizar")]
        public async Task<ActionResult<ApiResponse<PermisoDto>>> ActualizarPermiso(int id, [FromBody] PermisoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando permiso con ID {Id}.", id);
            // Solicita la actualización completa de la permiso.
            var response = await _PermisoService.ActualizarPermisoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Permiso.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<PermisoDto>>> UpdateParcialPermiso(int id, [FromBody] JsonPatchDocument<PermisoUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Permiso con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _PermisoService.ActualizarParcialPermisoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }
    }
}


