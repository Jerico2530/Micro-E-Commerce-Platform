using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Dto.OrdenDetalle
{
    public class OrdenDetalleUpdateDto
    {


        public int ProductoId { get; set; } // Si quieres permitir cambio de producto
        public int Cantidad { get; set; }
        public bool Estado { get; set; } = true; // Para activar/desactivar detalle

    }
}
