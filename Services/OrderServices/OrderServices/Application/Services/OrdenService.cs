using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using OrderServices.Api.Helpers;
using OrderServices.Api.Responses;
using OrderServices.Application.Helpers;
using OrderServices.Application.Services.IServices;
using OrderServices.Domain.Dto.Orden;
using OrderServices.Domain.Dto.OrdenDetalle;
using OrderServices.Domain.Entities;
using OrderServices.Infrastructure.Datos;
using OrderServices.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace OrderServices.Application.Services
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepositorio _OrdenRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdenService> _logger;
        private readonly IValidator<OrdenCreateDto> _createValidator;
        private readonly IValidator<OrdenUpdateDto> _updateValidator;
        private readonly IValidator<OrdenUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;
        private readonly ProductServiceClient _productClient;
        private readonly IOrdenDetalleRepositorio _OrdenDetalleRepo;

        public OrdenService(IOrdenRepositorio OrdenRepo, IMapper mapper, ILogger<OrdenService> logger, IValidator<OrdenCreateDto> createValidator, IValidator<OrdenUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<OrdenUpdateDto> patchValidator, ProductServiceClient productClient, IOrdenDetalleRepositorio OrdenDetalleRepo)
        {
            _OrdenRepo = OrdenRepo;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _patchValidator = patchValidator;
            _productClient = productClient;
            _OrdenDetalleRepo = OrdenDetalleRepo;

        }

        public async Task<ApiResponse<List<OrdenDto>>> ObtenerTodosLosOrdenAsync()
        {
            try
            {
                // 1️⃣ Traemos todas las órdenes con sus detalles
                var ordenes = await _OrdenRepo.ObtenerTodo(o => true); // o si tienes filtros, los pones aquí

                // 2️⃣ Traemos los detalles de cada orden desde la base de datos
                foreach (var orden in ordenes)
                {
                    // Asumiendo que tienes relación Orden -> OrdenDetalle en EF
                    var detalles = await _OrdenDetalleRepo.ObtenerTodo(d => d.OrdenId == orden.OrdenId);

                    orden.Total = detalles.Sum(d => d.SubTotal); // Calcula el total
                }

                // 3️⃣ Mapear a DTO
                var ordenesDto = _mapper.Map<List<OrdenDto>>(ordenes);

                return ResponseHelper.Success(ordenesDto, "Órdenes obtenidas exitosamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes: {Mensaje}", ex.Message);
                return ResponseHelper.Fail<List<OrdenDto>>($"Error al obtener órdenes: {ex.Message}", code: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ApiResponse<OrdenDto>> ObtenerOrdenPorIdAsync(int id)
        {
            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<OrdenDto>(validation.Errors);

                var Orden = await _OrdenRepo.ObtenerTodo(a => a.OrdenId == id);
                if (Orden == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Orden con ID {Id}.", id);
                    return ResponseHelper.Fail<OrdenDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Orden con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<OrdenDto>(Orden);
                _logger.LogInformation("✅ Orden con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Orden encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Orden por ID {Id}", id);
                return ResponseHelper.FailException<OrdenDto>(ex);
            }
        }

        public async Task<ApiResponse<OrdenDto>> CrearOrdenAsync(OrdenCreateDto dto, int usuarioId, string token)
        {
            try
            {
                // 1️⃣ Validar DTO
                var validation = await _createValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<OrdenDto>(validation.Errors);

                await using var transaction = await _OrdenRepo.BeginTransactionAsync();

                // 2️⃣ Crear la orden principal
                var orden = new Orden
                {
                    UsuarioId = usuarioId,
                    FechaOrden = DateTime.UtcNow,
                    Estado = true,
                    EstadoOrden = "Pendiente",
                    Total = 0m
                };

                await _OrdenRepo.Crear(orden);
                await _OrdenRepo.Grabar(); // EF genera OrdenId automáticamente

                decimal totalOrden = 0m;

                // 3️⃣ Crear detalles de la orden
                foreach (var item in dto.Items)
                {
                    // Verificar stock
                    bool stockDisponible = await _productClient.VerificarStockAsync(item.ProductoId, item.Cantidad, token);
                    if (!stockDisponible)
                    {
                        await transaction.RollbackAsync();
                        return ResponseHelper.Fail<OrdenDto>(
                            $"Stock insuficiente para producto {item.ProductoId}",
                            "Stock",
                            HttpStatusCode.BadRequest);
                    }

                    // Obtener precio
                    var price = await _productClient.ObtenerPrecioAsync(item.ProductoId, token);
                    if (price <= 0)
                    {
                        await transaction.RollbackAsync();
                        return ResponseHelper.Fail<OrdenDto>(
                            $"Precio inválido para producto {item.ProductoId}.",
                            "Price",
                            HttpStatusCode.BadRequest);
                    }

                    // Reducir stock
                    await _productClient.ReducirStockAsync(item.ProductoId, item.Cantidad, token);

                    // Crear detalle
                    var detalle = new OrdenDetalle
                    {
                        OrdenId = orden.OrdenId,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = price,
                        SubTotal = item.Cantidad * price,
                        Estado = true,
                        FechaRegistro = DateTime.UtcNow
                    };

                    totalOrden += detalle.SubTotal;
                    await _OrdenDetalleRepo.Crear(detalle);
                }

                // 4️⃣ Actualizar total de la orden
                orden.Total = totalOrden;
                await _OrdenRepo.ActualizarOrden(orden);

                // 5️⃣ Confirmar transacción
                await transaction.CommitAsync();

                var resultDto = _mapper.Map<OrdenDto>(orden);
                _logger.LogInformation("✅ Orden creada correctamente con ID {Id} y Total {Total}", orden.OrdenId, orden.Total);

                return ResponseHelper.Success(resultDto, "Orden creada correctamente", HttpStatusCode.Created);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "❌ Error de comunicación con ProductService");
                return ResponseHelper.Fail<OrdenDto>($"Error de comunicación con ProductService: {ex.Message}", code: HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear la orden");
                return ResponseHelper.FailException<OrdenDto>(ex);
            }
        }
        public async Task<ApiResponse<object>> EliminarOrdenAsync(int id)
        {
            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var Orden = await _OrdenRepo.Obtener(a => a.OrdenId == id);
                if (Orden == null)
                    return ResponseHelper.Fail<object>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

                await _OrdenRepo.Remover(Orden);
                _logger.LogInformation("✅ Orden ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Orden eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Orden ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }

        public async Task<ApiResponse<OrdenDto>> ActualizarOrdenAsync(int id, OrdenUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<OrdenDto>("Datos inválidos para actualizar Orden.", "Orden");

                var OrdenExistente = await _OrdenRepo.Obtener(a => a.OrdenId == id, tracked: true);
                if (OrdenExistente == null)
                    return ResponseHelper.Fail<OrdenDto>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<OrdenDto>(validation.Errors);

                _mapper.Map(updateDto, OrdenExistente);
                await _OrdenRepo.ActualizarOrden(OrdenExistente);

                _logger.LogInformation("✅ Orden ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<OrdenDto>(null, "Orden actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar Orden ID {Id}", id);
                return ResponseHelper.FailException<OrdenDto>(ex);
            }
        }

        public async Task<ApiResponse<OrdenDto>> ActualizarParcialOrdenAsync(int id, JsonPatchDocument<OrdenUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<OrdenDto>("Datos inválidos para la actualización parcial.", "Patch");

                var OrdenExistente = await _OrdenRepo.Obtener(a => a.OrdenId == id, tracked: true);
                if (OrdenExistente == null)
                    return ResponseHelper.Fail<OrdenDto>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<OrdenUpdateDto>(OrdenExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<OrdenDto>(validation.Errors);

                _mapper.Map(dto, OrdenExistente);
                await _OrdenRepo.ActualizarOrden(OrdenExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Orden ID {Id}.", id);
                return ResponseHelper.Success<OrdenDto>(null, "Orden actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Orden ID {Id}", id);
                return ResponseHelper.FailException<OrdenDto>(ex);
            }
        }
    }
}
