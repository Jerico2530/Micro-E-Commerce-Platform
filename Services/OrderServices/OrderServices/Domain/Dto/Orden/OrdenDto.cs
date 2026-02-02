using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Dto.Orden
{
    public class OrdenDto
    {
        public int OrdenId { get; set; }

        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaOrden { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string EstadoOrden { get; set; } = "Pendiente";

        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
