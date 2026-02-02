using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderServices.Api.Mapping;
using OrderServices.Application.Helpers;
using OrderServices.Application.Services;
using OrderServices.Application.Services.IServices;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Dto.OrdenDetalle;
using OrderServices.Domain.Validacion.Orden;
using OrderServices.Domain.Validacion.OrdenDetalle;
using OrderServices.Infrastructure.Datos;
using OrderServices.Infrastructure.Repositories;
using OrderServices.Infrastructure.Repositories.Interfaces;
using OrderServices.Infrastructure.Security;
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

// FluentValidation

builder.Services.AddScoped<IValidator<OrdenCreateDto>, OrdenCreateValidacion>();
builder.Services.AddScoped<IValidator<OrdenUpdateDto>, OrdenUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenGetValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDeleteValidacion>();

builder.Services.AddScoped<IValidator<OrdenDetalleCreateDto>, OrdenDetalleCreateValidacion>();
builder.Services.AddScoped<IValidator<OrdenDetalleUpdateDto>, OrdenDetalleUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDetalleGetValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDetalleDeleteValidacion>();



// Repositorios

builder.Services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
builder.Services.AddScoped<IOrdenDetalleRepositorio, OrdenDetalleRepositorio>();


// Servicios

builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IOrdenDetalleService, OrdenDetalleService>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, DynamicPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddHttpClient<ProductServiceClient>();


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

builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProductService:BaseUrl"]);
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
