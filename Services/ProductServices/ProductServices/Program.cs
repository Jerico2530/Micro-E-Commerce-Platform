using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductServices.Api.Mapping;
using ProductServices.Application.Services;
using ProductServices.Application.Services.IServices;
using ProductServices.Domain.Dto;
using ProductServices.Domain.Validacion.Producto;
using ProductServices.Infrastructure.Datos;
using ProductServices.Infrastructure.Repositories;
using ProductServices.Infrastructure.Repositories.Interfaces;
using ProductServices.Infrastructure.Security;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 🔧 SERVICIOS
// =========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// DbContext PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// FluentValidation dinámico
builder.Services.AddScoped<IValidator<ProductoCreateDto>, ProductoCreateValidacion>();
builder.Services.AddScoped<IValidator<ProductoUpdateDto>, ProductoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoDeleteValidacion>();

// Repositorios genéricos y específicos
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

// Servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();


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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),

        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


// =========================
// 🔧 Configurar Kestrel para Docker
// =========================
builder.WebHost.ConfigureKestrel(options =>
{
    // Escuchar en todas las interfaces en el puerto 80
    options.ListenAnyIP(80);
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

// ⚠️ Si estás en Docker usando HTTP, comentar HTTPS temporalmente
// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
