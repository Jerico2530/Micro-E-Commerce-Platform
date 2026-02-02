using AutoMapper;
using FluentValidation;
using IdentityServices.Api.Dto;
using IdentityServices.Api.Helpers;
using IdentityServices.Api.Responses;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Usuario;
using IdentityServices.Domain.Entities;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using IdentityServices.Security.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace IdentityServices.Application.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginService> _logger;

        private readonly JwtService _utilidades;
        private readonly PasswordHasher _passwordHasher;
        private readonly IValidator<UsuarioLoginDto> _loginValidator;


        public LoginService(
            IUsuarioRepositorio usuarioRepo, IMapper mapper, ILogger<LoginService> logger, JwtService utilidades, IValidator<UsuarioLoginDto> loginValidator,  PasswordHasher passwordHasher)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _logger = logger;
            _utilidades = utilidades;
            _loginValidator = loginValidator;
            _passwordHasher = passwordHasher;
        }


        // ==============================
        // LOGIN DE USUARIO NORMAL
        // ==============================
        public async Task<ApiResponse<LoginResultDto>> LoginAsync(UsuarioLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("🟡 Intentando login para: {Correo}", loginDto?.Email);

                var validation = await _loginValidator.ValidateAsync(loginDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<LoginResultDto>(validation.Errors);

                var usuario = await _usuarioRepo.Obtener(
                    u => u.Email == loginDto.Email && u.Estado,
                    include: q => q.Include(x => x.UserRoles)
                                   .ThenInclude(ur => ur.Rol)
                );

                if (usuario == null)
                    return ResponseHelper.Fail<LoginResultDto>(
                        "Usuario no encontrado o inactivo.",
                        "Email",
                        HttpStatusCode.Unauthorized);

                if (!_passwordHasher.VerificarPassword(loginDto.Contraseña, usuario.Contraseña))
                    return ResponseHelper.Fail<LoginResultDto>(
                        "Contraseña incorrecta.",
                        "Contraseña",
                        HttpStatusCode.Unauthorized);

                // Generar JWT
                string token = _utilidades.GenerarJWT(usuario.UsuarioId);

                var roles = usuario.UserRoles
                    .Where(ur => ur.Estado && ur.Rol.Estado)
                    .Select(ur => ur.Rol.NombreRol)
                    .ToList();

                var permisos = usuario.UserRoles
                   .SelectMany(ur => ur.Rol.PermRoles)
                   .Where(rp => rp.Estado && rp.Permiso.Estado)
                   .Select(rp => rp.Permiso.NombrePermiso)
                   .Distinct()
                   .ToList();

                var resultado = new LoginResultDto
                {
                    Token = token,
                    Usuario = new UsuarioInfoDto
                    {
                        UsuarioId = usuario.UsuarioId,
                        NombreCompleto = usuario.NombreCompleto,
                        ApellidoCompleto = usuario.ApellidoCompleto,
                        Email = usuario.Email,
                        Roles = roles,
                        Permisos = permisos
                    }
                };

                _logger.LogInformation("✅ Login exitoso para usuario: {Correo}", loginDto.Email);
                return ResponseHelper.Success(resultado, "Login exitoso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante login de usuario.");
                return ResponseHelper.FailException<LoginResultDto>(ex);
            }
        }

        // ==============================
        // LOGIN DE INVITADO
        // ==============================
        public async Task<ApiResponse<LoginResultDto>> LoginInvitadoAsync()
        {
            try
            {
                _logger.LogInformation("🟡 Intentando login de invitado.");

                var usuarioInvitado = await _usuarioRepo.Obtener(
                    u => u.Estado && u.UserRoles.Any(ur => ur.Rol.NombreRol == "Invitado"),
                    include: q => q.Include(x => x.UserRoles)
                                   .ThenInclude(ur => ur.Rol)
                );

                if (usuarioInvitado == null)
                    return ResponseHelper.Fail<LoginResultDto>(
                        "Usuario invitado no encontrado.",
                        "UsuarioId",
                        HttpStatusCode.NotFound);

                string token = _utilidades.GenerarJWT(usuarioInvitado.UsuarioId);

                var roles = usuarioInvitado.UserRoles
                    .Where(ur => ur.Estado && ur.Rol.Estado)
                    .Select(ur => ur.Rol.NombreRol)
                    .ToList();
                var permisos = usuarioInvitado.UserRoles
                    .SelectMany(ur => ur.Rol.PermRoles)
                    .Where(rp => rp.Estado && rp.Permiso.Estado)
                    .Select(rp => rp.Permiso.NombrePermiso)
                    .Distinct()
                    .ToList();

                var resultado = new LoginResultDto
                {
                    Token = token,
                    Usuario = new UsuarioInfoDto
                    {
                        UsuarioId = usuarioInvitado.UsuarioId,
                        NombreCompleto = usuarioInvitado.NombreCompleto,
                        ApellidoCompleto = usuarioInvitado.ApellidoCompleto,
                        Email = usuarioInvitado.Email,
                        Roles = roles,
                        Permisos = permisos
                    }
                };

                _logger.LogInformation("✅ Login de invitado exitoso (UsuarioId: {Id})", usuarioInvitado.UsuarioId);
                return ResponseHelper.Success(resultado, "Login de invitado exitoso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante login de invitado.");
                return ResponseHelper.FailException<LoginResultDto>(ex);
            }
        }
    }
}
