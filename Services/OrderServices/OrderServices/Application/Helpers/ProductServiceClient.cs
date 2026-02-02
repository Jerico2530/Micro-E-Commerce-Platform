using OrderServices.Api.Responses;
using OrderServices.Domain.Dto.Orden;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderServices.Application.Helpers
{
    // Cliente para comunicarse con ProductService usando JWT dinámico
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignora mayúsculas/minúsculas
            };
        }

        // --- Verificar stock de un producto ---
        public async Task<bool> VerificarStockAsync(int productoId, int cantidad, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"stock/{productoId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje = $"Error HTTP al consultar stock: {(int)response.StatusCode} {response.ReasonPhrase}";
                throw new Exception($"Error de comunicación con ProductService para producto {productoId}: {mensaje}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<StockResponseDto>>(_jsonOptions);

            if (apiResponse == null || !apiResponse.IsExitoso || apiResponse.Resultado == null)
                throw new Exception($"Respuesta inválida de ProductService para producto {productoId}");

            Console.WriteLine($"Stock actual del producto {productoId}: {apiResponse.Resultado.StockActual}");
            return apiResponse.Resultado.StockActual >= cantidad;
        }

        // --- Reducir stock ---
        public async Task<bool> ReducirStockAsync(int productoId, int cantidad, string token)
        {
            var dto = new StockRequestDto { ProductoId = productoId, Cantidad = cantidad };
            var request = new HttpRequestMessage(HttpMethod.Post, "stock/reducir")
            {
                Content = JsonContent.Create(dto)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var mensaje = $"Error HTTP al reducir stock: {(int)response.StatusCode} {response.ReasonPhrase}";
                throw new Exception($"Error de comunicación con ProductService para producto {productoId}: {mensaje}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<StockResponseDto>>(_jsonOptions);

            if (apiResponse == null || !apiResponse.IsExitoso || apiResponse.Resultado == null)
                throw new Exception($"Respuesta inválida de ProductService al reducir stock para producto {productoId}");

            return true;
        }

        // --- Obtener price de producto ---
        public async Task<decimal> ObtenerPrecioAsync(int productoId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"producto/{productoId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error de comunicación con ProductService para producto {productoId}");

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductPriceDto>>(_jsonOptions);

            if (apiResponse == null || !apiResponse.IsExitoso || apiResponse.Resultado == null)
                throw new Exception($"Respuesta inválida de ProductService al obtener price para producto {productoId}");

            Console.WriteLine($"Precio recibido para producto {productoId}: {apiResponse.Resultado.Price}"); // 🔹 debug

            return apiResponse.Resultado.Price;
        }

        // --- DTOs ---
        public class ProductPriceDto { public decimal Price { get; set; } }
        public class StockResponseDto
        {
            public int ProductoId { get; set; }
            public int StockActual { get; set; }
            public bool Disponible { get; set; }
        }
        public class StockRequestDto
        {
            public int ProductoId { get; set; }
            public int Cantidad { get; set; }
        }
    }
}