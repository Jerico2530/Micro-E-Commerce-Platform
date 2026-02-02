using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Entities
{
    public class Orden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrdenId { get; set; }

        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaOrden { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string EstadoOrden { get; set; } = "Pendiente";

        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<OrdenDetalle> Detalles { get; set; } = new List<OrdenDetalle>();
    }
}
