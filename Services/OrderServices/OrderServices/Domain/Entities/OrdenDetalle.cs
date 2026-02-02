using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Entities
{
    public class OrdenDetalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrdenDetalleId { get; set; }

        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public int ProductoId { get; set; }

        [Required]
        public int Cantidad { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        public bool Estado { get; set; } = true;
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
