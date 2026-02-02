using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Dto.Orden
{
    public class OrdenUpdateDto
    {

        public string EstadoOrden { get; set; } = "Pendiente"; // Ej: Pendiente, Completada
        public bool Estado { get; set; } = true; // Para activar/desactivar la orden

    }
}
