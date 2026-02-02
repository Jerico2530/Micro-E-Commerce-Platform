using AutoMapper;
using FluentValidation;
using IdentityServices.Api.Helpers;
using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Usuario;
using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServices.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepositorio _UsuarioRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IValidator<UsuarioCreateDto> _createValidator;
        private readonly IValidator<UsuarioUpdateDto> _updateValidator;
        private readonly IValidator<UsuarioUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;




        public UsuarioService(IUsuarioRepositorio usuarioRepo, IMapper mapper, ILogger<UsuarioService> logger, AppDbContext context, IValidator<UsuarioCreateDto> createValidator, IValidator<UsuarioUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<UsuarioUpdateDto> patchValidator)
        {
            _UsuarioRepo = usuarioRepo;
            _mapper = mapper;
            _logger = logger;
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _patchValidator = patchValidator;

        }

        public async Task<ApiResponse<List<UsuarioDto>>> ObtenerTodosLosUsuarioAsync()
        {
            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los Usuarios activos...");

                var Usuarios = await _UsuarioRepo.ObtenerTodo();

                if (Usuarios == null || !Usuarios.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron Usuarios registrados.");
                    return ResponseHelper.Fail<List<UsuarioDto>>(
                        new List<ErrorDetail> { new() { Campo = "Usuarios", Mensaje = "No se encontraron Usuarios registrados." } },
                        HttpStatusCode.NotFound
                    );
                }

                var UsuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(Usuarios).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} Usuarios.", UsuariosDto.Count);
                return ResponseHelper.Success(UsuariosDto, "Usuarios obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Usuarios.");
                return ResponseHelper.FailException<List<UsuarioDto>>(ex);
            }
        }


        public async Task<ApiResponse<UsuarioDto>> ObtenerUsuarioPorIdAsync(int id)
        {


            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

                var Usuario = await _UsuarioRepo.Obtener(a => a.UsuarioId == id);
                if (Usuario == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Usuario con ID {Id}.", id);
                    return ResponseHelper.Fail<UsuarioDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Usuario con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<UsuarioDto>(Usuario);
                _logger.LogInformation("✅ Usuario con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Usuario encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Usuario por ID {Id}", id);
                return ResponseHelper.FailException<UsuarioDto>(ex);
            }
        }

        public async Task<ApiResponse<UsuarioDto>> CrearUsuarioAsync(UsuarioCreateDto createDto)
        {
            try
            {
                // 🔹 Validación inicial del DTO
                if (createDto == null)
                    return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para crear usuario.", "Usuario");

                // 🔹 Validación con FluentValidation
                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

                // 🔹 Validación defensiva
                if (string.IsNullOrWhiteSpace(createDto.Email))
                    return ResponseHelper.Fail<UsuarioDto>("El Email es obligatorio.", "Email");
                if (string.IsNullOrWhiteSpace(createDto.Contraseña))
                    return ResponseHelper.Fail<UsuarioDto>("La contraseña es obligatoria.", "Contraseña");
                if (createDto.Contraseña != createDto.ContraseñaVisible)
                    return ResponseHelper.Fail<UsuarioDto>("Las contraseñas no coinciden.", "Contraseña");

                // 🔹 Verificar existencia del usuario
                var existe = await _UsuarioRepo.Obtener(u =>
                    u.Email != null && u.Email.ToLower() == createDto.Email.ToLower()
                );
                if (existe != null)
                    return ResponseHelper.Fail<UsuarioDto>("Ya existe un usuario con ese Email.", "Email", HttpStatusCode.Conflict);

                // 🔹 Hashear contraseña
                createDto.Contraseña = BCrypt.Net.BCrypt.HashPassword(createDto.Contraseña, workFactor: 6);

                // 🔹 Mapear DTO a entidad
                var usuario = _mapper.Map<Usuario>(createDto);

                // 🔹 Preparar rol "Usuario" y relación UserRol en una sola transacción
                // Usamos Transacción para optimizar SaveChanges
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Crear usuario
                await _UsuarioRepo.Crear(usuario);


                // Obtener rol o crear solo si no existe
                var rolUsuario = await _context.Roles
                    .FirstOrDefaultAsync(r => r.NombreRol == "USUARIO");

                if (rolUsuario == null)
                {
                    rolUsuario = new Rol { NombreRol = "USUARIO", Estado = true };
                    _context.Roles.Add(rolUsuario);
                }

                // Crear relación UserRol
                var userRol = new UserRol
                {
                    UsuarioId = usuario.UsuarioId,
                    RolId = rolUsuario.RolId,
                    Estado = true
                };
                _context.UserRoles.Add(userRol);

                // 🔹 Guardar todos los cambios en una sola llamada
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 🔹 Mapear a DTO de respuesta
                var dto = _mapper.Map<UsuarioDto>(usuario);

                return ResponseHelper.Success(dto, "Usuario creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario en CrearUsuarioAsync");

                return ResponseHelper.Fail<UsuarioDto>(
                    ex.Message,
                    "Exception",
                    HttpStatusCode.InternalServerError
                );
            }

        }

        public async Task<ApiResponse<object>> EliminarUsuarioAsync(int id)
        {
            try
            {

                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var usuario = await _UsuarioRepo.Obtener(p => p.UsuarioId == id);
                if (usuario == null)
                    return ResponseHelper.Fail<object>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

                // Eliminar
                await _UsuarioRepo.Remover(usuario);

                _logger.LogInformation("✅ Usuario ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Usuario eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Usuario ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }



        public async Task<ApiResponse<UsuarioDto>> ActualizarUsuarioAsync(int id, UsuarioUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para actualizar usuario.", "Usuario");

                var usuarioExistente = await _UsuarioRepo.Obtener(u => u.UsuarioId == id, tracked: true);
                if (usuarioExistente == null)
                    return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.OK);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

                _mapper.Map(updateDto, usuarioExistente);
                await _UsuarioRepo.ActualizarUsuario(usuarioExistente);

                _logger.LogInformation("✅ Usuario ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<UsuarioDto>(null, "Usuario actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar usuario ID {Id}", id);
                return ResponseHelper.FailException<UsuarioDto>(ex);
            }
        }



        public async Task<ApiResponse<UsuarioDto>> ObtenerUsuarioActualAsync(int id)
        {
            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

                var usuario = await _UsuarioRepo.Obtener(u => u.UsuarioId == id && u.Estado);
                if (usuario == null)
                    return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<UsuarioDto>(usuario);
                _logger.LogInformation("✅ Usuario con ID {Id} obtenido correctamente.", id);

                return ResponseHelper.Success(dto, "Usuario obtenido correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener usuario ID {Id}", id);
                return ResponseHelper.FailException<UsuarioDto>(ex);
            }
        }


        public async Task<ApiResponse<UsuarioDto>> ActualizarParcialUsuarioAsync(int id, JsonPatchDocument<UsuarioUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para la actualización parcial.", "Patch");

                var UsuarioExistente = await _UsuarioRepo.Obtener(a => a.UsuarioId == id, tracked: true);
                if (UsuarioExistente == null)
                    return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<UsuarioUpdateDto>(UsuarioExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

                _mapper.Map(dto, UsuarioExistente);
                await _UsuarioRepo.ActualizarUsuario(UsuarioExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Usuario ID {Id}.", id);
                return ResponseHelper.Success<UsuarioDto>(null, "Usuario actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Usuario ID {Id}", id);
                return ResponseHelper.FailException<UsuarioDto>(ex);
            }
        }
    }
}
