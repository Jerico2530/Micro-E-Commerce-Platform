using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using ProductServices.Api.Helpers;
using ProductServices.Api.Responses;
using ProductServices.Application.Services.IServices;
using ProductServices.Domain.Dto;
using ProductServices.Domain.Entities;
using ProductServices.Infrastructure.Datos;
using ProductServices.Infrastructure.Repositories.Interfaces;
using System.Net;

namespace ProductServices.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepositorio _ProductoRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductoCreateDto> _createValidator;
        private readonly IValidator<ProductoUpdateDto> _updateValidator;
        private readonly IValidator<ProductoUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;
        private readonly ILogger<ProductoService> _logger;



        public ProductoService(IProductoRepositorio productoRepo, IMapper mapper, ILogger<ProductoService> logger, IValidator<ProductoCreateDto> createValidator, IValidator<ProductoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<ProductoUpdateDto> patchValidator)
        {
            _ProductoRepo = productoRepo;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _patchValidator = patchValidator;
            _logger = logger;

        }

        public async Task<ApiResponse<List<ProductoDto>>> ObtenerTodosLosProductosAsync()
        {

            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los productos...");

                var productos = await _ProductoRepo.ObtenerTodo();

                if (productos == null || !productos.Any())
                {
                    return ResponseHelper.Fail<List<ProductoDto>>(
                        "No se encontraron productos registrados.",
                        "Productos",
                        HttpStatusCode.NotFound);
                }

                var productosDto = _mapper.Map<List<ProductoDto>>(productos);

                return ResponseHelper.Success(productosDto, "Productos obtenidos correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener productos");
                return ResponseHelper.FailException<List<ProductoDto>>(ex);
            }
        }

        public async Task<ApiResponse<ProductoDto>> ObtenerProductoPorIdAsync(int id)
        {

            try
            {
                var idValidation = await _getValidator.ValidateAsync(id);
                if (!idValidation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(idValidation.Errors);

                var producto = await _ProductoRepo.Obtener(p => p.ProductoId == id);

                if (producto == null)
                {
                    return ResponseHelper.Fail<ProductoDto>(
                        "Producto no encontrado.",
                        "Id",
                        HttpStatusCode.NotFound);
                }

                var dto = _mapper.Map<ProductoDto>(producto);
                return ResponseHelper.Success(dto, "Producto encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener producto ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }


        public async Task<ApiResponse<ProductoDto>> CrearProductoAsync(ProductoCreateDto createDto)
        {

            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<ProductoDto>("Datos inválidos.", "Producto");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                var producto = _mapper.Map<Producto>(createDto);
                producto.Estado = true; // controlado desde backend

                await _ProductoRepo.Crear(producto);

                var dto = _mapper.Map<ProductoDto>(producto);
                return ResponseHelper.Success(dto, "Producto creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear producto");
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarProductoAsync(int id)
        {

            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var producto = await _ProductoRepo.Obtener(p => p.ProductoId == id);
                if (producto == null)
                    return ResponseHelper.Fail<object>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                await _ProductoRepo.Remover(producto);

                return ResponseHelper.Success<object>(null, "Producto eliminado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar producto ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }


        public async Task<ApiResponse<ProductoDto>> ActualizarProductoAsync(int id, ProductoUpdateDto updateDto)
        {

            try
            {
                var idValidation = await _getValidator.ValidateAsync(id);
                if (!idValidation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(idValidation.Errors);

                if (updateDto == null)
                    return ResponseHelper.Fail<ProductoDto>("Datos inválidos.", "Producto");

                var producto = await _ProductoRepo.Obtener(p => p.ProductoId == id, tracked: true);
                if (producto == null)
                    return ResponseHelper.Fail<ProductoDto>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                _mapper.Map(updateDto, producto);
                await _ProductoRepo.ActualizarProducto(producto);

                var dto = _mapper.Map<ProductoDto>(producto);
                return ResponseHelper.Success(dto, "Producto actualizado correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar producto ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }

        public async Task<ApiResponse<ProductoDto>> ActualizarParcialProductoAsync(int id, JsonPatchDocument<ProductoUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null)
                    return ResponseHelper.Fail<ProductoDto>("Patch inválido.", "Patch");

                var producto = await _ProductoRepo.Obtener(p => p.ProductoId == id, tracked: true);
                if (producto == null)
                    return ResponseHelper.Fail<ProductoDto>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<ProductoUpdateDto>(producto);

                try
                {
                    patchDto.ApplyTo(dto);
                }
                catch (Exception ex)
                {
                    return ResponseHelper.Fail<ProductoDto>(ex.Message, "Patch");
                }

                var validation = await _patchValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                _mapper.Map(dto, producto);
                await _ProductoRepo.ActualizarProducto(producto);

                var resultDto = _mapper.Map<ProductoDto>(producto);
                return ResponseHelper.Success(resultDto, "Producto actualizado parcialmente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al producto ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }
    }
}

