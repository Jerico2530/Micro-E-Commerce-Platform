using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderServices.Domain.Dto.OrdenDetalle
{
    public class OrdenDetalleCreateDto
    {

        public int ProductoId { get; set; }
        public int Cantidad { get; set; }

        public bool Estado { get; set; } = true; 

    }
}
