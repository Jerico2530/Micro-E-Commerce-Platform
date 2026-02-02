using AutoMapper;
using FluentValidation;
using IdentityServices.Api.Helpers;
using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Rol;
using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using System.Net;

namespace IdentityServices.Application.Services
{
    public class RolService : IRolService
    {
        private readonly IRolRepositorio _RolRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<RolService> _logger;
        private readonly AppDbContext _context;
        private readonly IValidator<RolCreateDto> _createValidator;
        private readonly IValidator<RolUpdateDto> _updateValidator;
        private readonly IValidator<RolUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;



        public RolService(IRolRepositorio rolRepo, IMapper mapper, ILogger<RolService> logger, IValidator<RolCreateDto> createValidator, IValidator<RolUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<RolUpdateDto> patchValidator, AppDbContext context)
        {
            _RolRepo = rolRepo;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _context = context;
            _patchValidator = patchValidator;

        }

        public async Task<ApiResponse<List<RolDto>>> ObtenerTodosLosRolAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los Rols activos...");

                var Rols = await _RolRepo.ObtenerTodo();

                if (Rols == null || !Rols.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron Rols registrados.");
                    return ResponseHelper.Fail<List<RolDto>>(
                        new List<ErrorDetail> { new() { Campo = "Rols", Mensaje = "No se encontraron Rols registrados." } },
                        HttpStatusCode.NotFound
                    );
                }

                var RolsDto = _mapper.Map<IEnumerable<RolDto>>(Rols).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} Rols.", RolsDto.Count);
                return ResponseHelper.Success(RolsDto, "Rols obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Rols.");
                return ResponseHelper.FailException<List<RolDto>>(ex);
            }
        }

        public async Task<ApiResponse<RolDto>> ObtenerRolPorIdAsync(int id)
        {
            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<RolDto>(validation.Errors);

                var Rol = await _RolRepo.Obtener(a => a.RolId == id);
                if (Rol == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Rol con ID {Id}.", id);
                    return ResponseHelper.Fail<RolDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Rol con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<RolDto>(Rol);
                _logger.LogInformation("✅ Rol con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Rol encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Rol por ID {Id}", id);
                return ResponseHelper.FailException<RolDto>(ex);
            }
        }

        public async Task<ApiResponse<RolDto>> CrearRolAsync(RolCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<RolDto>("Datos inválidos para crear Rol.", "Rol");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<RolDto>(validation.Errors);

                var existe = await _RolRepo.Obtener(a => a.NombreRol.ToLower() == createDto.NombreRol.ToLower());
                if (existe != null)
                    return ResponseHelper.Fail<RolDto>("Ya existe un Rol con ese NombreRol.", "NombreRol", HttpStatusCode.Conflict);

                var modelo = _mapper.Map<Rol>(createDto);
                await _RolRepo.Crear(modelo);

                var dto = _mapper.Map<RolDto>(modelo);
                _logger.LogInformation("✅ Rol '{NombreRol}' creado correctamente.", dto.NombreRol);
                return ResponseHelper.Success(dto, "Rol creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear Rol.");
                return ResponseHelper.FailException<RolDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarRolAsync(int id)
        {
            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var Rol = await _RolRepo.Obtener(a => a.RolId == id);
                if (Rol == null)
                    return ResponseHelper.Fail<object>("Rol no encontrado.", "Id", HttpStatusCode.NotFound);

                await _RolRepo.Remover(Rol);
                _logger.LogInformation("✅ Rol ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Rol eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Rol ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }

        public async Task<ApiResponse<RolDto>> ActualizarRolAsync(int id, RolUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<RolDto>("Datos inválidos para actualizar Rol.", "Rol");

                var RolExistente = await _RolRepo.Obtener(a => a.RolId == id, tracked: true);
                if (RolExistente == null)
                    return ResponseHelper.Fail<RolDto>("Rol no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<RolDto>(validation.Errors);

                _mapper.Map(updateDto, RolExistente);
                await _RolRepo.ActualizarRol(RolExistente);

                _logger.LogInformation("✅ Rol ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<RolDto>(null, "Rol actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar Rol ID {Id}", id);
                return ResponseHelper.FailException<RolDto>(ex);
            }
        }

        public async Task<ApiResponse<RolDto>> ActualizarParcialRolAsync(int id, JsonPatchDocument<RolUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<RolDto>("Datos inválidos para la actualización parcial.", "Patch");

                var RolExistente = await _RolRepo.Obtener(a => a.RolId == id, tracked: true);
                if (RolExistente == null)
                    return ResponseHelper.Fail<RolDto>("Rol no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<RolUpdateDto>(RolExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<RolDto>(validation.Errors);

                _mapper.Map(dto, RolExistente);
                await _RolRepo.ActualizarRol(RolExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Rol ID {Id}.", id);
                return ResponseHelper.Success<RolDto>(null, "Rol actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Rol ID {Id}", id);
                return ResponseHelper.FailException<RolDto>(ex);
            }
        }
    }
}
