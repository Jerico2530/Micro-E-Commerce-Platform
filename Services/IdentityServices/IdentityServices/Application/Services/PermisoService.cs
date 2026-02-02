using AutoMapper;
using FluentValidation;
using IdentityServices.Api.Helpers;
using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Permiso;
using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using System.Net;

namespace IdentityServices.Application.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepositorio _PermisoRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PermisoService> _logger;
        private readonly IValidator<PermisoCreateDto> _createValidator;
        private readonly IValidator<PermisoUpdateDto> _updateValidator;
        private readonly IValidator<PermisoUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;
        private readonly AppDbContext _context;




        public PermisoService(IPermisoRepositorio PermisoRepo, IMapper mapper, ILogger<PermisoService> logger, IValidator<PermisoCreateDto> createValidator, IValidator<PermisoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PermisoUpdateDto> patchValidator, AppDbContext context)
        {
            _PermisoRepo = PermisoRepo;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _patchValidator = patchValidator;
            _context = context;
        }

        public async Task<ApiResponse<List<PermisoDto>>> ObtenerTodosLosPermisoAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los Permisos activos...");

                var Permisos = await _PermisoRepo.ObtenerTodo();

                if (Permisos == null || !Permisos.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron Permisos registrados.");
                    return ResponseHelper.Fail<List<PermisoDto>>(
                        new List<ErrorDetail> { new() { Campo = "Permisos", Mensaje = "No se encontraron Permisos registrados." } },
                        HttpStatusCode.NotFound
                    );
                }

                var PermisosDto = _mapper.Map<IEnumerable<PermisoDto>>(Permisos).OrderBy(a => a.NombrePermiso).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} Permisos.", PermisosDto.Count);
                return ResponseHelper.Success(PermisosDto, "Permisos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Permisos.");
                return ResponseHelper.FailException<List<PermisoDto>>(ex);
            }
        }

        public async Task<ApiResponse<PermisoDto>> ObtenerPermisoPorIdAsync(int id)
        {
            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<PermisoDto>(validation.Errors);

                var Permiso = await _PermisoRepo.Obtener(a => a.PermisoId == id);
                if (Permiso == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Permiso con ID {Id}.", id);
                    return ResponseHelper.Fail<PermisoDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Permiso con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<PermisoDto>(Permiso);
                _logger.LogInformation("✅ Permiso con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Permiso encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Permiso por ID {Id}", id);
                return ResponseHelper.FailException<PermisoDto>(ex);
            }
        }


        public async Task<ApiResponse<PermisoDto>> CrearPermisoAsync(PermisoCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<PermisoDto>("Datos inválidos para crear Permiso.", "Permiso");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<PermisoDto>(validation.Errors);

                var existe = await _PermisoRepo.Obtener(a => a.NombrePermiso.ToLower() == createDto.NombrePermiso.ToLower());
                if (existe != null)
                    return ResponseHelper.Fail<PermisoDto>("Ya existe un Permiso con ese NombrePermiso.", "NombrePermiso", HttpStatusCode.Conflict);

                var modelo = _mapper.Map<Permiso>(createDto);
                await _PermisoRepo.Crear(modelo);

                var dto = _mapper.Map<PermisoDto>(modelo);
                _logger.LogInformation("✅ Permiso '{NombrePermiso}' creado correctamente.", dto.NombrePermiso);
                return ResponseHelper.Success(dto, "Permiso creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear Permiso.");
                return ResponseHelper.FailException<PermisoDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarPermisoAsync(int id)
        {
            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var Permiso = await _PermisoRepo.Obtener(a => a.PermisoId == id);
                if (Permiso == null)
                    return ResponseHelper.Fail<object>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

                await _PermisoRepo.Remover(Permiso);
                _logger.LogInformation("✅ Permiso ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Permiso eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Permiso ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }

        public async Task<ApiResponse<PermisoDto>> ActualizarPermisoAsync(int id, PermisoUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<PermisoDto>("Datos inválidos para actualizar Permiso.", "Permiso");

                var PermisoExistente = await _PermisoRepo.Obtener(a => a.PermisoId == id, tracked: true);
                if (PermisoExistente == null)
                    return ResponseHelper.Fail<PermisoDto>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<PermisoDto>(validation.Errors);

                _mapper.Map(updateDto, PermisoExistente);
                await _PermisoRepo.ActualizarPermiso(PermisoExistente);

                _logger.LogInformation("✅ Permiso ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<PermisoDto>(null, "Permiso actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar Permiso ID {Id}", id);
                return ResponseHelper.FailException<PermisoDto>(ex);
            }
        }



        public async Task<ApiResponse<PermisoDto>> ActualizarParcialPermisoAsync(int id, JsonPatchDocument<PermisoUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<PermisoDto>("Datos inválidos para la actualización parcial.", "Patch");

                var PermisoExistente = await _PermisoRepo.Obtener(a => a.PermisoId == id, tracked: true);
                if (PermisoExistente == null)
                    return ResponseHelper.Fail<PermisoDto>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<PermisoUpdateDto>(PermisoExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<PermisoDto>(validation.Errors);

                _mapper.Map(dto, PermisoExistente);
                await _PermisoRepo.ActualizarPermiso(PermisoExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Permiso ID {Id}.", id);
                return ResponseHelper.Success<PermisoDto>(null, "Permiso actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Permiso ID {Id}", id);
                return ResponseHelper.FailException<PermisoDto>(ex);
            }

        }
    }
}
