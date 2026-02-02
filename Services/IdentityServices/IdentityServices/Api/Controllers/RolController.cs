using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Rol;
using IdentityServices.Security.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServices.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly ILogger<RolController> _logger;
        private readonly IRolService _RolService;

        public RolController(ILogger<RolController> logger, IRolService RolService)
        {
            _logger = logger;
            _RolService = RolService;
        }
        [HttpGet]
        [AutorizacionPermiso("Rol.Ver")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<RolDto>>>> GetRol()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Rols");
            // Llama la capa de servicios para obtener el listado de rols.
            var response = await _RolService.ObtenerTodosLosRolAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("{id:int}", Name = "GetRol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Rol.VerDetalle")]
        public async Task<ActionResult<ApiResponse<RolDto>>> GetRol(int id)
        {
            _logger.LogInformation("🔍 Solicitando Rol con ID {RolId}.", id);
            // Consulta al servicio por el detalle de la rol solicitada.
            var response = await _RolService.ObtenerRolPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Rol.Crear")]
        public async Task<ActionResult<ApiResponse<RolDto>>> CrearRol([FromBody] RolCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Rol.");
            // Solicita la creación de la rol en la capa de servicios.
            var response = await _RolService.CrearRolAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Rol: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetRol", new { id = carrito?.RolId }, response);

        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Rol.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarRol(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Rol con ID {RolId}", id);
            // Solicita al servicio eliminar la rol indicada.
            var response = await _RolService.EliminarRolAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Rol.Actualizar")]
        public async Task<ActionResult<ApiResponse<RolDto>>> ActualizarRol(int id, [FromBody] RolUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando rol con ID {Id}.", id);
            // Solicita la actualización completa de la rol.
            var response = await _RolService.ActualizarRolAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }




        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Rol.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<RolDto>>> UpdateParcialRol(int id, JsonPatchDocument<RolUpdateDto> patchDto)
        {

            _logger.LogInformation("🧩 Actualización parcial de Rol con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _RolService.ActualizarParcialRolAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
