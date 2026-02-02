using FluentValidation;
using IdentityServices.Api.Mapping;
using IdentityServices.Application.Services;
using IdentityServices.Application.Services.IServices;
using IdentityServices.Domain.Dto.Rol;
using IdentityServices.Domain.Dto.UserRol;
using IdentityServices.Domain.Dto.Usuario;
using IdentityServices.Domain.Validacion.Rol;
using IdentityServices.Domain.Validacion.UserRol;
using IdentityServices.Domain.Validacion.Usuario;
using IdentityServices.Infrastructure.Datos;
using IdentityServices.Infrastructure.Repositories;
using IdentityServices.Infrastructure.Repositories.Interfaces;
using IdentityServices.Security.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// DbContext PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation

builder.Services.AddScoped<IValidator<RolCreateDto>, RolCreateValidacion>();
builder.Services.AddScoped<IValidator<RolUpdateDto>, RolUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, RolGetValidacion>();
builder.Services.AddScoped<IValidator<int>, RolDeleteValidacion>();

builder.Services.AddScoped<IValidator<UserRolCreateDto>, UserRolCreateValidacion>();
builder.Services.AddScoped<IValidator<UserRolUpdateDto>, UserRolUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, UserRolGetValidacion>();
builder.Services.AddScoped<IValidator<int>, UserRolDeleteValidacion>();

builder.Services.AddScoped<IValidator<UsuarioCreateDto>, UsuarioCreateValidacion>();
builder.Services.AddScoped<IValidator<UsuarioUpdateDto>, UsuarioUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, UsuarioGetValidacion>();
builder.Services.AddScoped<IValidator<int>, UsuarioDeleteValidacion>();

builder.Services.AddScoped<IValidator<UsuarioLoginDto>, LoginCreateValidacion>();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordHasher>();

// Repositorios

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IRolRepositorio, RolRepositorio>();
builder.Services.AddScoped<IUserRolRepositorio, UserRolRepositorio>();

// Servicios

builder.Services.AddScoped<IUserRolService, UserRolService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddAuthorization();


// CORS escalable (permitir solo los dominios que necesites)
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()   // En producción: cambiar por tus dominios
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =========================
// 🔧 Configurar Kestrel para Docker
// =========================
builder.WebHost.ConfigureKestrel(options =>
{
    // Escuchar en todas las interfaces en el puerto 80
    options.ListenAnyIP(80);
});

// JWT Authentication
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});


var app = builder.Build();

// =========================
// 🌐 MIDDLEWARE
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
        c.RoutePrefix = string.Empty; // http://localhost:5001/
    });
}

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
